using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueTools.Utils;

public unsafe class Shop
{
    private readonly Dictionary<string, ShopPurchaseSession> activeSessions = new();
    
    private static EventFramework* EF => EventFramework.Instance();
    private static AgentShop* AS => AgentShop.Instance();
    private static GameObjectManager* GOM => GameObjectManager.Instance();
    private static TargetSystem* TS => TargetSystem.Instance();

    public bool? ProcessPurchase(ulong vendorId, uint shopId, (uint itemId, int count)[] items)
    {
        var sessionKey = $"{vendorId}_{shopId}";
        
        if (!activeSessions.TryGetValue(sessionKey, out var session))
        {
            session = new ShopPurchaseSession(vendorId, shopId, items);
            activeSessions[sessionKey] = session;
        }

        var result = session.Process();
        
        if (result != false) // Completed (true) or failed (null)
        {
            activeSessions.Remove(sessionKey);
        }
        
        return result;
    }

    public void ClearAllSessions()
    {
        activeSessions.Clear();
        PluginLog.Debug("Cleared all shop sessions");
    }

    public bool? BuyFromShop(ulong vendorInstanceId, uint shopId, uint itemId, int count)
    {
        return BuyFromShop(vendorInstanceId, shopId, [(itemId, count)]);
    }

    public bool? BuyFromShop(ulong vendorInstanceId, uint shopId, (uint itemId, int count)[] items)
    {
        return ProcessPurchase(vendorInstanceId, shopId, items);
    }

    public void ClearShopStates()
    {
        ClearAllSessions();
    }

    private class ShopPurchaseSession
    {
        private readonly ulong vendorId;
        private readonly uint shopId;
        private readonly (uint itemId, int count)[] items;
        private int currentItemIndex;
        private SessionState state;

        private enum SessionState
        {
            Opening,
            WaitingForShopOpen,
            WaitingForEvent,
            PurchasingItem,
            WaitingForTransaction,
            Closing,
            WaitingForClose
        }

        public ShopPurchaseSession(ulong vendorId, uint shopId, (uint itemId, int count)[] items)
        {
            this.vendorId = vendorId;
            this.shopId = shopId;
            this.items = items;
            this.currentItemIndex = 0;
            this.state = SessionState.Opening;
        }

        public bool? Process()
        {
            try
            {
                return state switch
                {
                    SessionState.Opening => ProcessOpening(),
                    SessionState.WaitingForShopOpen => ProcessWaitingForShopOpen(),
                    SessionState.WaitingForEvent => ProcessWaitingForEvent(),
                    SessionState.PurchasingItem => ProcessPurchasingItem(),
                    SessionState.WaitingForTransaction => ProcessWaitingForTransaction(),
                    SessionState.Closing => ProcessClosing(),
                    SessionState.WaitingForClose => ProcessWaitingForClose(),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Shop session error: {ex.Message}");
                return null;
            }
        }

        private bool? ProcessOpening()
        {
            if (IsShopOpen(shopId))
            {
                state = SessionState.WaitingForEvent;
                return false;
            }

            if (EzThrottler.Throttle($"ShopOpen_{vendorId}_{shopId}", 2000))
            {
                if (!TryOpenShop())
                {
                    PluginLog.Error($"Failed to open shop {vendorId:X}.{shopId:X}");
                    return null;
                }
            }

            state = SessionState.WaitingForShopOpen;
            return false;
        }

        private bool? ProcessWaitingForShopOpen()
        {
            if (!IsShopOpen(shopId)) return false;
            
            state = SessionState.WaitingForEvent;
            return false;
        }

        private bool? ProcessWaitingForEvent()
        {
            if (!Svc.Condition[ConditionFlag.OccupiedInEvent]) return false;
            
            state = SessionState.PurchasingItem;
            return false;
        }

        private bool? ProcessPurchasingItem()
        {
            if (currentItemIndex >= items.Length)
            {
                state = SessionState.Closing;
                return false;
            }

            var (itemId, count) = items[currentItemIndex];
            PluginLog.Information($"Purchasing item {currentItemIndex + 1}/{items.Length}: {count}x {itemId}");

            if (TryPurchaseItem(itemId, count))
            {
                state = SessionState.WaitingForTransaction;
            }
            else
            {
                PluginLog.Warning($"Failed to purchase {count}x {itemId}, skipping");
                currentItemIndex++;
            }
            
            return false;
        }

        private bool? ProcessWaitingForTransaction()
        {
            if (IsTransactionInProgress()) return false;
            
            currentItemIndex++;
            state = SessionState.PurchasingItem;
            return false;
        }

        private bool? ProcessClosing()
        {
            if (!TryCloseShop())
            {
                PluginLog.Error($"Failed to close shop {vendorId:X}.{shopId:X}");
                return null;
            }
            
            state = SessionState.WaitingForClose;
            return false;
        }

        private bool? ProcessWaitingForClose()
        {
            if (IsShopOpen() || Svc.Condition[ConditionFlag.OccupiedInEvent]) return false;
            
            var itemNames = string.Join(", ", items.Select(x => $"{x.count}x{x.itemId}"));
            PluginLog.Information($"Successfully completed purchase of {itemNames}");
            return true;
        }

        private bool TryOpenShop()
        {
            var vendor = GOM->Objects.GetObjectByGameObjectId(vendorId);
            if (vendor == null) return false;

            TS->InteractWithObject(vendor);
            var selector = EventHandlerSelector.Instance();
            
            if (selector->Target == null) return true;
            if (selector->Target != vendor) return false;

            for (int i = 0; i < selector->OptionsCount; ++i)
            {
                if (selector->Options[i].Handler->Info.EventId.Id == shopId)
                {
                    EF->InteractWithHandlerFromSelector(i);
                    return true;
                }
            }
            
            return false;
        }

        private bool TryPurchaseItem(uint itemId, int count)
        {
            if (!EF->EventHandlerModule.EventHandlerMap.TryGetValuePointer(shopId, out var eh) || 
                eh == null || eh->Value == null) return false;

            if (eh->Value->Info.EventId.ContentId != EventHandlerContent.Shop) return false;

            var shop = (ShopEventHandler*)eh->Value;
            
            for (int i = 0; i < shop->VisibleItemsCount; ++i)
            {
                var index = shop->VisibleItems[i];
                if (shop->Items[index].ItemId == itemId)
                {
                    shop->BuyItemIndex = index;
                    shop->ExecuteBuy(count);
                    return true;
                }
            }
            
            return false;
        }

        private bool TryCloseShop()
        {
            if (AS == null || AS->EventReceiver == null) return false;

            AtkValue res = default, arg = default;
            var proxy = (ShopEventHandler.AgentProxy*)AS->EventReceiver;
            proxy->Handler->CancelInteraction();
            arg.SetInt(-1);
            AS->ReceiveEvent(&res, &arg, 1, 0);
            return true;
        }

        private bool IsTransactionInProgress()
        {
            if (!EF->EventHandlerModule.EventHandlerMap.TryGetValuePointer(shopId, out var eh) || 
                eh == null || eh->Value == null) return false;

            if (eh->Value->Info.EventId.ContentId != EventHandlerContent.Shop) return false;

            var shop = (ShopEventHandler*)eh->Value;
            return shop->WaitingForTransactionToFinish;
        }
    }

    private static bool IsShopOpen(uint shopId = 0)
    {
        if (AS == null || !AS->IsAgentActive() || AS->EventReceiver == null || !AS->IsAddonReady())
            return false;
        if (shopId == 0) return true;
        if (!EF->EventHandlerModule.EventHandlerMap.TryGetValuePointer(shopId, out var eh) || eh == null || eh->Value == null)
            return false;
        var proxy = (ShopEventHandler.AgentProxy*)AS->EventReceiver;
        return proxy->Handler == eh->Value;
    }
} 