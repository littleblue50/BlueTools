using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace BlueTools.Utils;

public unsafe class PlayerHelper
{
    public bool IsGatherer()
    {
        Job playerJobId = (Job)(Svc.ClientState.LocalPlayer?.ClassJob.Value.RowId ?? 0);

        if (playerJobId != Job.FSH && playerJobId != Job.MIN && playerJobId != Job.BTN)
        {
            return false;
        }

        return true;
    }

    public bool SwitchToJob(Job job)
    {
        if (Svc.ClientState.LocalPlayer?.ClassJob.Value.RowId == (uint)job)
        {
            return true;
        }

        // TODO: Implement gearset switching
        return false;
    }
}