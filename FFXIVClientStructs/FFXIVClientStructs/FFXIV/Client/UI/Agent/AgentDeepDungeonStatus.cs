using FFXIVClientStructs.FFXIV.Client.System.String;

namespace FFXIVClientStructs.FFXIV.Client.UI.Agent;

// Client::UI::Agent::AgentDeepDungeonStatus
//   Client::UI::Agent::AgentInterface
//     Component::GUI::AtkModuleInterface::AtkEventInterface
[Agent(AgentId.DeepDungeonStatus)]
[GenerateInterop]
[Inherits<AgentInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe partial struct AgentDeepDungeonStatus {
    [FieldOffset(0x28)] public DeepDungeonStatusData* Data;
}

[GenerateInterop]
[StructLayout(LayoutKind.Explicit, Size = 0x8D8)]
public unsafe partial struct DeepDungeonStatusData {
    [FieldOffset(0x00)] public uint Level;
    [FieldOffset(0x04)] public uint MaxLevel;
    [FieldOffset(0x08)] public uint ClassJobId;

    [FieldOffset(0x18), FixedSizeArray] internal FixedSizeArray16<DeepDungeonStatusItem> _pomander;
    [FieldOffset(0x718), FixedSizeArray] internal FixedSizeArray4<DeepDungeonStatusItem> _magicite;
}

[StructLayout(LayoutKind.Explicit, Size = 0x70)]
public struct DeepDungeonStatusItem {
    [FieldOffset(0x00)] public uint ItemId; // DeepDungeonItem for Pomander, DeepDungeonMagicStone for Magicite
    [FieldOffset(0x04)] public uint Icon;
    [FieldOffset(0x08)] public Utf8String Name;
}
