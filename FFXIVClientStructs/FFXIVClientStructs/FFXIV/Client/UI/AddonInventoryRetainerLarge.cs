using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonInventoryRetainerLarge
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("InventoryRetainerLarge")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x320)]
public partial struct AddonInventoryRetainerLarge {
    [FieldOffset(0x270)] public AtkAddonControl AddonControl;

    [FieldOffset(0x308)] public int TabIndex;

    [MemberFunction("E9 ?? ?? ?? ?? 33 D2 E8 ?? ?? ?? ?? 48 83 C4 48")]
    public partial void SetTab(int tab);
}
