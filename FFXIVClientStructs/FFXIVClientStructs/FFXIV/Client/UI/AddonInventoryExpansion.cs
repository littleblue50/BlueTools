using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonInventoryExpansion
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("InventoryExpansion")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x348)]
public partial struct AddonInventoryExpansion {
    [FieldOffset(0x298)] public AtkAddonControl AddonControl;

    [FieldOffset(0x340)] public int TabIndex;

    [MemberFunction("E8 ?? ?? ?? ?? BB ?? ?? ?? ?? 83 EB 01")]
    public partial void SetTab(int tab, bool force);
}
