using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonGSInfoEmj
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("GSInfoEmj")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x2A8)]
public unsafe partial struct AddonGSInfoEmj {
    [FieldOffset(0x238)] public AtkTextNode* MatchesPlayed;
    [FieldOffset(0x240)] public AtkTextNode* CurrentRating;
    [FieldOffset(0x248)] public AtkTextNode* HighestRating;
    [FieldOffset(0x250)] public AtkTextNode* Rank;
    [FieldOffset(0x258)] public AtkTextNode* NextRankPoints;
    [FieldOffset(0x260)] public AtkTextNode* Points;
    [FieldOffset(0x268)] public AtkComponentButton* ResetRankButton;
}
