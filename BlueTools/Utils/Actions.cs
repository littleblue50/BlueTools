using BlueTools.Enums;
using BlueTools.Services;
using ECommons.Automation;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace BlueTools.Utils;

public unsafe class Actions
{

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