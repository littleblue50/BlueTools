using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonSocial
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("Social")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x330)]
public unsafe partial struct AddonSocial {
    [FieldOffset(0x248)] public AtkAddonControl AddonControl;
    [FieldOffset(0x2A8)] public AtkComponentRadioButton* PartyMembersRadioButton;
    [FieldOffset(0x2B0)] public AtkComponentRadioButton* FriendListRadioButton;
    [FieldOffset(0x2B8)] public AtkComponentRadioButton* BlacklistRadioButton;
    [FieldOffset(0x2C0)] public AtkComponentRadioButton* PlayerSearchRadioButton;
    [FieldOffset(0x2C8)] public Utf8String PlayerSearchString; // idk, it's literally just the words "Player Search"
}
