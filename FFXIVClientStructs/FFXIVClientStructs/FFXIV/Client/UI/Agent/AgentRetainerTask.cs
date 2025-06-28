namespace FFXIVClientStructs.FFXIV.Client.UI.Agent;

// Client::UI::Agent::AgentRetainerTask
//   Client::UI::Agent::AgentInterface
//     Component::GUI::AtkModuleInterface::AtkEventInterface
[Agent(AgentId.RetainerTask)]
[GenerateInterop]
[Inherits<AgentInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x90)]
public unsafe partial struct AgentRetainerTask {
    // 00 - None
    // 01 - Request Assignment
    // 02 - Venture in Progress
    // 03 - Completed Venture
    [FieldOffset(0x28)] public uint DisplayType;

    [FieldOffset(0x38)] public uint OpenerAddonId;

    // Set when venture is complete
    [FieldOffset(0x44)] public uint RewardXP;

    // Set when venture is complete
    [FieldOffset(0x50), FixedSizeArray] internal FixedSizeArray2<uint> _rewardItemIds;

    // Set when venture is complete
    [FieldOffset(0x58), FixedSizeArray] internal FixedSizeArray2<uint> _rewardItemCount;

    [FieldOffset(0x6C)] public uint RetainerTaskLvRange;

    [FieldOffset(0x74)] public uint RetainerTaskId;

    [FieldOffset(0x80)] public bool IsLoading;

    // Set when venture in progress
    [FieldOffset(0x84)] public uint XPToReward;
}
