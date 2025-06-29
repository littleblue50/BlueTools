using BlueTools.Enums;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace BlueTools.Utils;

public unsafe class Fishing
{
    public static bool QuitFishing()
    {
        if (Svc.Condition[ConditionFlag.ExecutingGatheringAction]) return false;
        if (!Svc.Condition[ConditionFlag.Fishing]) return true;

        return ActionManager.Instance()->UseAction(ActionType.Action, 299);
    }

    public static bool StartFishing()
    {
        if (Svc.Condition[ConditionFlag.Fishing]) return true;
        if (Player.IsBusy || Player.Mounted) return false;

        return ActionManager.Instance()->UseAction(ActionType.Action, 289);
    }

    public static bool ChangeBait(BaitType baitType)
    {
        var currentBait = PlayerState.Instance()->FishingBait;
        
        if ((uint)baitType == currentBait)
        {
            return true;
        }
        
        var inventoryCount = InventoryManager.Instance()->GetInventoryItemCount((uint)baitType);
        if (inventoryCount == 0)
        {
            PluginLog.Debug($"Bait {baitType} not found in inventory");
            return false;
        }

        var baitName = ExcelItemHelper.GetName((uint)baitType);
        PluginLog.Debug($"Attempting to change bait to {baitName} using /bait command");
        
        try
        {
            Chat.ExecuteCommand($"/bait {baitName}");
            // Return false to indicate we need to wait and check on next tick
            // Don't return true immediately since the command is async
            return false;
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Failed to execute bait change command: {ex.Message}");
            return false;
        }
    }
}