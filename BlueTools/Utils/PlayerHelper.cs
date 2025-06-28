using ECommons.ExcelServices;
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

    public bool ChangeJob(Job job)
    {
        if (Svc.ClientState.LocalPlayer?.ClassJob.Value.RowId == (uint)job)
        {
            return true;
        }

        // Get the RaptureGearsetModule instance
        var gearsetModule = RaptureGearsetModule.Instance();
        if (gearsetModule == null)
        {
            return false;
        }

        // Look for a gearset with the specified job
        for (int i = 0; i < gearsetModule->NumGearsets; i++)
        {
            if (!gearsetModule->IsValidGearset(i))
                continue;

            var gearset = gearsetModule->GetGearset(i);
            if (gearset == null)
                continue;

            if (gearset->Flags.HasFlag(RaptureGearsetModule.GearsetFlag.MainHandMissing))
                continue;

            if (gearset->Flags.HasFlag(RaptureGearsetModule.GearsetFlag.Exists) && 
                gearset->ClassJob == (byte)job)
            {
                var result = gearsetModule->EquipGearset(i);
                return result == 0;
            }
        }

        return false;
    }
}