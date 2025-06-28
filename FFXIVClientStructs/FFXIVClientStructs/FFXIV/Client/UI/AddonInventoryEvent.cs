using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonInventoryEvent
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("InventoryEvent")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x328)]
public unsafe partial struct AddonInventoryEvent {
    [FieldOffset(0x270), FixedSizeArray] internal FixedSizeArray5<Pointer<AtkComponentRadioButton>> _buttons;

    [FieldOffset(0x2A8)] public AtkAddonControl AddonControl;

    [FieldOffset(0x320)] public int TabIndex;

    [MemberFunction("E8 ?? ?? ?? ?? EB 09 83 FF 01")]
    public partial void SetTab(int tab);
}
