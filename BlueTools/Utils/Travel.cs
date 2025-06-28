using BlueTools.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace BlueTools.Utils;

public unsafe class Travel
{
    public static void Dismount()
    {
        if (!Player.Mounted) return;

        if (EzThrottler.Throttle("DismountAction", 1000))
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 23);
        }
    }

    public static void Mount()
    {
        if (Player.Mounted || Player.Mounting) return;

        if (EzThrottler.Throttle("MountAction", 1000))
        {
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 24);
        }
    }

    public static bool MoveToPosition(Vector3 targetPosition, bool pathOnGround = false, bool dismount = false)
    {
        if (Player.IsBusy) return false;

        if (!Service.IPC.NavMeshIPC.IsReady() || 
            Service.IPC.NavMeshIPC.IsRunning() || 
            Service.IPC.NavMeshIPC.PathfindInProgress()) return false;

        // If we're in the process of mounting/dismounting, wait
        if (Player.Mounting)
        {
            return false;
        }

        float distance = Player.DistanceTo(targetPosition);

        if (Player.Mounted && pathOnGround)
        {
            Dismount();
            return false;
        }

        if (distance <= 0.5f)
        {
            if (Player.Mounted && dismount)
            {
                Dismount();
                return false;
            }

            return true;
        }
        
        // If we're far away and can mount, try to mount first
        if (distance > 10.0f && !Player.Mounted && Player.CanMount && !pathOnGround)
        {
            Mount();
            return false;
        }
        
        PluginLog.Debug($"Moving to {targetPosition.X:F1}, {targetPosition.Y:F1}, {targetPosition.Z:F1} (distance: {distance:F1})");
        Service.IPC.NavMeshIPC.PathfindAndMoveTo(targetPosition, Player.Mounted && !pathOnGround);

        return false;
    }
}