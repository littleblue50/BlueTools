using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace BlueTools.Utils;

public unsafe class Fishing
{
    public static bool QuitFishing()
    {
        if (!Svc.Condition[ConditionFlag.Fishing]) return true;

        return ActionManager.Instance()->UseAction(ActionType.Action, 299);
    }

    public static bool StartFishing()
    {
        if (Svc.Condition[ConditionFlag.Fishing]) return true;
        if (Player.IsBusy || Player.Mounted) return false;

        return ActionManager.Instance()->UseAction(ActionType.Action, 289);
    }
}