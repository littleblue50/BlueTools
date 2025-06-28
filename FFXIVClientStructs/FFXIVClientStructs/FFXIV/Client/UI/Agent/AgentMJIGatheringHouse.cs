using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace FFXIVClientStructs.FFXIV.Client.UI.Agent;

// Client::UI::Agent::AgentMJIGatheringHouse
//   Client::UI::Agent::AgentInterface
//     Component::GUI::AtkModuleInterface::AtkEventInterface
[Agent(AgentId.MJIGatheringHouse)]
[GenerateInterop]
[Inherits<AgentInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x200)]
public unsafe partial struct AgentMJIGatheringHouse {
    public enum Confirmation : int { None, Start, ChangeExtend, Change, Extend }

    [FieldOffset(0x028)] public MJIManager* Manager;
    [FieldOffset(0x030)] public MJIGranariesState* GranariesState;
    [FieldOffset(0x038)] public AgentData* Data;
    [FieldOffset(0x040)] public StringsData Strings;
    [FieldOffset(0x178)] public int ConfirmAddonHandle;
    [FieldOffset(0x17C)] public byte CurGranaryIndex;
    // 0x180: item row id of 'cowrie' while waiting for it to be available
    [FieldOffset(0x188)] public Utf8String CurExpeditionName;
    [FieldOffset(0x1F0)] public byte CurActiveExpeditionId;
    [FieldOffset(0x1F1)] public byte CurSelectedExpeditionId;
    [FieldOffset(0x1F2)] public byte CurHoveredExpeditionId;
    [FieldOffset(0x1F3)] public byte CurActiveDays;
    [FieldOffset(0x1F4)] public byte CurSelectedDays;
    [FieldOffset(0x1F8)] public int SelectExpeditionAddonHandle;
    [FieldOffset(0x1FC)] public Confirmation ConfirmType;

    [GenerateInterop]
    [StructLayout(LayoutKind.Explicit, Size = 0x138)]
    public unsafe partial struct StringsData {
        [FieldOffset(0x00)] public Utf8String ConfirmText;
        [FieldOffset(0x68), FixedSizeArray] internal FixedSizeArray2<Utf8String> _finishTimeText;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public unsafe partial struct AgentData {
        [FieldOffset(0x00)] public AgentMJIGatheringHouse* Owner;
        // 0x08: sheets[2]
        [FieldOffset(0x38)] public StdVector<ExpeditionData> Expeditions;
        [FieldOffset(0x50)] public StdVector<ExpeditionDesc> ExpeditionDescs;
        [FieldOffset(0x68)] public StdVector<ExpeditionItem> ExpeditionItems;
        [FieldOffset(0x80)] public StdVector<Resource> Resources;
        [FieldOffset(0x98)] public StdVector<uint> ItemsPendingIconUpdate;
        [FieldOffset(0xB0)] public bool Initialized;
    }

    [GenerateInterop]
    [StructLayout(LayoutKind.Explicit, Size = 0x120)]
    public unsafe partial struct ExpeditionData {
        [FieldOffset(0x000)] public byte ExpeditionId;
        [FieldOffset(0x008)] public Utf8String Name;
        [FieldOffset(0x070), FixedSizeArray] internal FixedSizeArray20<uint> _normalItemIds;
        [FieldOffset(0x0C0), FixedSizeArray] internal FixedSizeArray20<uint> _normalIconIds;
        [FieldOffset(0x110)] public byte NumNormalItems;
        [FieldOffset(0x114)] public uint RareItemId;
        [FieldOffset(0x118)] public uint RareIconId;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public unsafe partial struct ExpeditionDesc {
        [FieldOffset(0x0)] public byte ExpeditionId;
        //[FieldOffset(0x1)] public byte u1;
        [FieldOffset(0x2)] public byte RarePouchId;
        //[FieldOffset(0x3)] public byte u3;
        //[FieldOffset(0x4)] public ushort u4;
        [FieldOffset(0x6)] public ushort NameId;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x6)]
    public unsafe partial struct ExpeditionItem {
        [FieldOffset(0x0)] public ushort ExpeditionId;
        //[FieldOffset(0x2)] public ushort u2;
        [FieldOffset(0x4)] public byte PouchId;
        [FieldOffset(0x5)] public byte u5;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xC)]
    public unsafe partial struct Resource {
        [FieldOffset(0x0)] public ushort PouchId;
        //[FieldOffset(0x2)] public ushort u2;
        [FieldOffset(0x4)] public uint ItemId;
        [FieldOffset(0x8)] public uint IconId;
    }

    [MemberFunction("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B FA 48 8B F1 48 85 D2 74 31")]
    public partial bool IsExpeditionUnlocked(ExpeditionData* expedition);
}
