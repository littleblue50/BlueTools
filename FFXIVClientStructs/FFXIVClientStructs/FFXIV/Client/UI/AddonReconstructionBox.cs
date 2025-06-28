using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonReconstructionBox
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("ReconstructionBox")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x458)]
public unsafe partial struct AddonReconstructionBox {
    [FieldOffset(0x270), FixedSizeArray] internal FixedSizeArray10<AddonItemDonationInfo> _donationInfos;

    [FieldOffset(0x450)] public int ItemHovered; // 1 if hovering an item, 0 otherwise
}

[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe struct AddonItemDonationInfo {
    [FieldOffset(0x00)] public void* UnkPtr1;
    [FieldOffset(0x08)] public void* UnkPtr2;
    [FieldOffset(0x10)] public void* UnkPtr3;
    [FieldOffset(0x18)] public void* UnkPtr4;
    [FieldOffset(0x20)] public void* UnkPtr5;
    [FieldOffset(0x2C)] public int UnkValue1;
}
