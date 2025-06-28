using BlueTools.Enums;
using BlueTools.Services;
using BlueTools.Data;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.ExcelServices;
using BlueTools.Utils;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace BlueTools.Modules;

public record ShopNPCInfo(Vector3 Position, ulong VendorId, uint ShopId);

public unsafe class DiademHelper : BaseModule
{
    public override string Name => "Diadem Helper";

    private InventoryManager* inventoryManager => InventoryManager.Instance();
    private ShopNPCInfo merchantNPC = new(new Vector3(-641.9315f,285.2844f,-138.1888f), 4303540818, 263016);
    private ulong diademEntryNPCObjectId = 4303233947;
    
    // All bait types we need to track
    private readonly BaitType[] requiredBaits = 
    {
        BaitType.DiademBalloonBug,
        BaitType.DiademCraneFly,
        BaitType.DiademHoverworm,
        BaitType.DiademRedBalloon
    };
    
    protected override void OnEnable()
    {
        PluginLog.Information($"{Name} module enabled");
        Service.Utils.Shop.ClearAllSessions(); // Clear any stuck shop states
    }
    
    protected override void OnDisable()
    {
        // Stop any ongoing pathfinding when disabled
        if (Service.IPC.NavMeshIPC.IsRunning())
        {
            Service.IPC.NavMeshIPC.Stop();
        }
        
        // Stop fishing directly without using TaskManager
        if (Svc.Condition[ConditionFlag.Fishing])
        {
            Fishing.QuitFishing();
        }
        
        // Clean up AutoHook presets
        Service.IPC.AutohookIPC.DeleteAllAnonymousPresets();
        
        // Also abort any TaskManager tasks
        P.TaskManager.Abort();
        
        // Clear shop states to prevent stuck states on restart
        Service.Utils.Shop.ClearAllSessions();
    }
    
    public override void Tick()
    {
        if (Service.ModuleManager.ActiveModule != this) return;
        if (!Player.Available) return;
        if (Player.Territory != (uint)TerritoryType.Diadem)
        {
            EnterDiadem();
            return;
        }
        
        if (!EzThrottler.Throttle("DiademHelperTick", 1000)) return;
        
        if (BlueTools.Config.DiademShouldFish)
        {
            Fish();
        }
        
        if (BlueTools.Config.DiademShouldGather)
        {
            Gather();
        }
    } 

    private void EnterDiadem()
    {
        if (Player.IsBusy) return;
        if (P.TaskManager.IsBusy) return;
        if (Service.IPC.NavMeshIPC.IsRunning()) return;
        if (Service.IPC.LifestreamIPC.IsBusy()) return;
        
        // Don't try to enter if we're already in a duty queue or transitioning
        if (Svc.Condition[ConditionFlag.BoundByDuty] || 
            Svc.Condition[ConditionFlag.WaitingForDuty] ||
            Svc.Condition[ConditionFlag.WaitingForDutyFinder]) return;
        
        if (Player.Job != Job.BTN && Player.Job != Job.FSH && Player.Job != Job.MIN)
        {
            bool changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.BTN);
            if (!changedJob)
            {
                changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.MIN);
            }
            if (!changedJob)
            {
                changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.FSH);
            }
            
            if (!changedJob)
            {
                PluginLog.Error("Failed to become a gatherer, disabling module");
                Disable();
                return;
            }

        }

        if (Player.Territory == (uint)TerritoryType.Firmament)
        {
            P.TaskManager.Enqueue(() => Travel.MoveToPosition(new Vector3(-19.187387f, -16f, 142.10204f)));
            
            var npc = Svc.Objects.First(x => x.GameObjectId == diademEntryNPCObjectId);
            P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => npc));
            
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SkipTalk());
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SelectListIndex(0));
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SelectYesno());
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.ConfirmContentsFinder());
        }
        else
        {
            PluginLog.Information("Teleporting to Foundation and then Firmament to enter Diadem");
            P.TaskManager.Enqueue(() => Service.IPC.LifestreamIPC.Teleport((uint)Aetheryte.Foundation, 0));
            P.TaskManager.Enqueue(() => Player.Territory == (uint)TerritoryType.Foundation && !Player.IsBusy);
            P.TaskManager.Enqueue(() => Service.IPC.LifestreamIPC.AethernetTeleportToFirmament());
        }
    }

    private void Gather()
    {
        // stub for now until implemented
    }
    
    private void Fish()
    {
        if (P.TaskManager.IsBusy) return;

        if (Player.Job != Job.FSH)
        {
            Service.Utils.PlayerHelper.ChangeJob(Job.FSH);
        }
        
        if (EzThrottler.Throttle("DiademAmissCheck", 500))
        {
            CheckForFishAmissMessage();
        }

        var currentWeather = Weather.GetCurrentWeather();
        var targetGrade = BlueTools.Config.DiademTargetGrade;

        if (!currentWeather.HasValue) return;

        var fishingPosition = DiademFish.GetFishingPosition(targetGrade, currentWeather.Value);
        var autohookPreset = DiademFish.GetAutohookPreset(targetGrade, currentWeather.Value);
        var requiredBait = DiademFish.GetBait(targetGrade, currentWeather.Value);
        
        if (fishingPosition == null || autohookPreset == null)
        {
            return;
        }

        // Check if we need to restock bait first
        if (NeedToRestockBait())
        {
            PluginLog.Information("Low bait detected, restocking at merchant");
            RestockBait();
            return;
        }

        // Check if we're in the right position
        if (!IsAtCorrectFishingPosition(targetGrade, currentWeather.Value))
        {
            PluginLog.Information($"Moving to fishing spot for {currentWeather.Value} Grade {targetGrade}");
            StopFishing();
            MoveToFishingPosition(fishingPosition, targetGrade, currentWeather.Value);
            return;
        }

        // Check if we have the right bait
        var equippedBait = (BaitType?)PlayerState.Instance()->FishingBait;
        if (requiredBait.HasValue && requiredBait.Value != equippedBait)
        {
            if (Svc.Condition[ConditionFlag.Fishing])
            {
                StopFishing();
                return;
            }
            
            if (EzThrottler.Throttle("BaitChangeAttempt", 2000))
            {
                PluginLog.Information($"Changing bait to {requiredBait.Value}");
                Actions.ChangeBait(requiredBait.Value);
            }
            return;
        }

        // Start fishing if not already fishing and ready
        var isFishing = Svc.Condition[ConditionFlag.Fishing];
        var isMounted = Player.Mounted;
        var navMeshRunning = Service.IPC.NavMeshIPC.IsRunning();
        var pathfindInProgress = Service.IPC.NavMeshIPC.PathfindInProgress();
        
        if (!isFishing && !isMounted && !navMeshRunning && !pathfindInProgress)
        {
            StartFishing(autohookPreset);
        }
    }

    private bool NeedToRestockBait()
    {
        foreach (var bait in requiredBaits)
        {
            var currentCount = inventoryManager->GetInventoryItemCount((uint)bait);
            if (currentCount == 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAtCorrectFishingPosition(int targetGrade, WeatherType currentWeather)
    {
        if (Player.Mounted) return false;

        var spotInfo = DiademFish.GetFishingSpot(targetGrade, currentWeather);
        if (spotInfo == null) return false;

        foreach (var pos in spotInfo.FishingPositions)
        {
            if (Player.DistanceTo(pos) <= 1.0f)
            {
                return true;
            }
        }
        return false;
    }

    private void MoveToFishingPosition(Vector3? fishingPosition, int targetGrade, WeatherType currentWeather)
    {
        if (Service.IPC.NavMeshIPC.IsRunning()) return;
        if (P.TaskManager.IsBusy) return;
        
        if (fishingPosition.HasValue)
        {
            var landingPosition = DiademFish.GetLandingPosition(targetGrade, currentWeather);
            
            if (landingPosition.HasValue)
            {
                var landingPos = landingPosition.Value;
                var fishingPos = fishingPosition.Value;
                var distanceToLanding = Player.DistanceTo(landingPos);
                
                // Only enqueue landing movement if we're not already there
                if (distanceToLanding > 1.0f)
                {
                    P.TaskManager.Enqueue(() => Travel.MoveToPosition(landingPos, false), "MoveLanding");
                }
                
                P.TaskManager.Enqueue(() => Travel.MoveToPosition(fishingPos, true), "MoveFishing");
            }
            else
            {
                var fishingPos = fishingPosition.Value;
                P.TaskManager.Enqueue(() => Travel.MoveToPosition(fishingPos), "MoveFishingDirect");
            }
        }
    }

    private void StartFishing(string autohookPreset)
    {
        if (P.TaskManager.IsBusy) return;
        
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.CreateAndSelectAnonymousPreset(autohookPreset));
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.SetPluginState(true));
        P.TaskManager.Enqueue(() => Fishing.StartFishing());
    }

    private void StopFishing()
    {
        if (!Svc.Condition[ConditionFlag.Fishing]) return;
        if (P.TaskManager.IsBusy) return;
        
        P.TaskManager.Enqueue(() => Fishing.QuitFishing());
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.DeleteAllAnonymousPresets());
    }

    private void CheckForFishAmissMessage()
    {
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("_TextError", out var addon) && addon->IsVisible)
        {
            var node = GenericHelpers.GetNodeByIDChain(addon->RootNode, 2);
            if (node != null && node->GetNodeType() == NodeType.Text)
            {
                var textNode = node->GetAsAtkTextNode();
                CheckMessageAndRotate(textNode->NodeText.ToString());
            }
        }
    }
    
    private void CheckMessageAndRotate(string nodeText)
    {
        // Check if the message contains the "fish sense something amiss" text
        if (nodeText.Contains("fish sense something amiss", StringComparison.OrdinalIgnoreCase) ||
            nodeText.Contains("fish here have grown wise", StringComparison.OrdinalIgnoreCase))
        {
            // Throttle to prevent multiple rotations for the same message
            if (EzThrottler.Throttle("FishAmissRotation", 10000)) // 10 second cooldown
            {
                PluginLog.Information($"Detected fish amiss message, rotating position!");
                RotateToNextFishingPosition();
            }
        }
    }

    /// <summary>
    /// Call this when you get the "fish sense something amiss" message to rotate to the next fishing spot
    /// </summary>
    public void RotateToNextFishingPosition()
    {
        if (P.TaskManager.IsBusy) return;
        
        var currentWeather = Weather.GetCurrentWeather();
        var targetGrade = BlueTools.Config.DiademTargetGrade;
        
        // We need a valid weather type for Diadem fishing
        if (!currentWeather.HasValue) return;
        
        PluginLog.Information("Fish sense something amiss - rotating to next fishing position");
        
        StopFishing();
        
        // Get the next fishing position and move there
        var nextPosition = DiademFish.GetNextFishingPosition(targetGrade, currentWeather.Value);
        MoveToFishingPosition(nextPosition, targetGrade, currentWeather.Value);
        
        // Start fishing at the new position
        var autohookPreset = DiademFish.GetAutohookPreset(targetGrade, currentWeather.Value);
        if (autohookPreset != null)
        {
            P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.CreateAndSelectAnonymousPreset(autohookPreset));
            P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.SetPluginState(true));
            P.TaskManager.Enqueue(() => Fishing.StartFishing());
        }
    }

    private void RestockBait()
    {
        if (P.TaskManager.IsBusy) return;

        // Check what items need restocking
        var itemsToBuy = new List<(uint itemId, int count)>();
        
        foreach (var bait in requiredBaits)
        {
            var currentCount = inventoryManager->GetInventoryItemCount((uint)bait);
            if (currentCount < BlueTools.Config.DiademBaitCount)
            {
                itemsToBuy.Add(((uint)bait, BlueTools.Config.DiademBaitCount - currentCount));
            }
        }
        
        // Only proceed if there are items to buy
        if (itemsToBuy.Count == 0) return;
        
        PluginLog.Information($"Restocking bait: {string.Join(", ", itemsToBuy.Select(x => $"{x.count}x{x.itemId}"))}");
        
        // Move to merchant first
        P.TaskManager.Enqueue(() => Travel.MoveToPosition(merchantNPC.Position));
        
        // Buy all items in one shop session
        P.TaskManager.Enqueue(() => Service.Utils.Shop.BuyFromShop(merchantNPC.VendorId, merchantNPC.ShopId, itemsToBuy.ToArray()));
    }

} 