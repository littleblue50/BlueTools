using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonJournalResult
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("JournalResult")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x2A0)]
public unsafe partial struct AddonJournalResult {
    [FieldOffset(0x238)] public AtkImageNode* AtkImageNode220;
    [FieldOffset(0x240)] public AtkImageNode* AtkImageNode228;
    [FieldOffset(0x248)] public AtkImageNode* AtkImageNode230;
    [FieldOffset(0x250)] public AtkComponentGuildLeveCard* AtkComponentGuildLeveCard238;
    [FieldOffset(0x258)] public AtkComponentButton* CompleteButton;
    [FieldOffset(0x260)] public AtkComponentButton* DeclineButton;
    [FieldOffset(0x268)] public AtkTextNode* AtkTextNode250;
    [FieldOffset(0x270)] public AtkTextNode* AtkTextNode258;
    [FieldOffset(0x278)] public AtkImageNode* AtkImageNode260;
    [FieldOffset(0x280)] public AtkComponentJournalCanvas* AtkComponentJournalCanvas268;
}
