using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonContextIconMenu
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x2C8)]
public unsafe partial struct AddonContextIconMenu {
    [FieldOffset(0x238)] public int EntryCount;
    [FieldOffset(0x258)] public AtkComponentList* AtkComponentList240;
    [FieldOffset(0x260)] public void* unk248;
    [FieldOffset(0x268)] public AtkComponentRadioButton* AtkComponentRadioButton250;
    [FieldOffset(0x270)] public AtkComponentRadioButton* AtkComponentRadioButton258;
    [FieldOffset(0x278)] public AtkComponentRadioButton* AtkComponentRadioButton260;
    [FieldOffset(0x280)] public AtkComponentRadioButton* AtkComponentRadioButton268;
    [FieldOffset(0x288)] public AtkComponentRadioButton* AtkComponentRadioButton270;
    [FieldOffset(0x290)] public AtkComponentRadioButton* AtkComponentRadioButton278;
    [FieldOffset(0x298)] public AtkComponentRadioButton* AtkComponentRadioButton280;
    [FieldOffset(0x2A0)] public AtkComponentRadioButton* AtkComponentRadioButton288;
    [FieldOffset(0x2A8)] public AtkComponentRadioButton* AtkComponentRadioButton290;
    [FieldOffset(0x2B0)] public AtkComponentRadioButton* AtkComponentRadioButton298;
}
