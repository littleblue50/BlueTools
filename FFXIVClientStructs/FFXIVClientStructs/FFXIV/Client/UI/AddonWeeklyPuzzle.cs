using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Component::GUI::AddonWeeklyPuzzle
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("WeeklyPuzzle")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0xD18)]
public unsafe partial struct AddonWeeklyPuzzle {
    [FieldOffset(0x238)] public RewardPanelItem RewardPanelCommander;
    [FieldOffset(0x260)] public RewardPanelItem RewardPanelCoffer;
    [FieldOffset(0x288)] public RewardPanelItem RewardPanelGiftBox;
    [FieldOffset(0x2B0)] public RewardPanelItem RewardPanelDualBlades;
    [FieldOffset(0x2D8)] public AtkComponentButton* AtkComponentButton2C0;
    [FieldOffset(0x2E0)] public AtkResNode* AtkResNode2C8;
    [FieldOffset(0x2E8)] public AtkTextNode* AtkTextNode2D0;
    [FieldOffset(0x2F0)] public AtkTextNode* AtkTextNode2D8;
    [FieldOffset(0x2F8)] public AtkResNode* AtkResNode2E0;
    [FieldOffset(0x300)] public AtkTextNode* AtkTextNode2E8;
    [FieldOffset(0x308)] public AtkTextNode* AtkTextNode2F0;
    [FieldOffset(0x310)] public GameTileBoard GameBoard;
    [FieldOffset(0xA50)] public AtkResNode* AtkResNodeA38;
    [FieldOffset(0xB60)] public Utf8String CommanderStr;
    [FieldOffset(0xBC8)] public Utf8String CofferStr;
    [FieldOffset(0xC30)] public Utf8String GiftBoxStr;
    [FieldOffset(0xC98)] public Utf8String DualBladesStr;

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public struct RewardPanelItem {
        [FieldOffset(0x0)] public AtkComponentBase* CompBase;
        [FieldOffset(0x8)] public AtkResNode* Res;
        [FieldOffset(0x10)] public AtkTextNode* NameText;
        [FieldOffset(0x18)] public AtkTextNode* RewardText;
        [FieldOffset(0x20)] public long Unk20;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x6C0)]
    public struct GameTileBoard {
        [FieldOffset(0x0)] public GameTileRow Row1;
        [FieldOffset(0x120)] public GameTileRow Row2;
        [FieldOffset(0x240)] public GameTileRow Row3;
        [FieldOffset(0x360)] public GameTileRow Row4;
        [FieldOffset(0x480)] public GameTileRow Row5;
        [FieldOffset(0x5A0)] public GameTileRow Row6;

        public GameTileRow this[int index] => index switch {
            0 => Row1,
            1 => Row2,
            2 => Row3,
            3 => Row4,
            4 => Row5,
            5 => Row6,
            _ => throw new ArgumentOutOfRangeException("Valid indexes are 0 through 5 inclusive.")
        };
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x120)]
    public struct GameTileRow {
        [FieldOffset(0x0)] public GameTileItem Col1;
        [FieldOffset(0x30)] public GameTileItem Col2;
        [FieldOffset(0x60)] public GameTileItem Col3;
        [FieldOffset(0x90)] public GameTileItem Col4;
        [FieldOffset(0xC0)] public GameTileItem Col5;
        [FieldOffset(0xF0)] public GameTileItem Col6;

        public GameTileItem this[int index] => index switch {
            0 => Col1,
            1 => Col2,
            2 => Col3,
            3 => Col4,
            4 => Col5,
            5 => Col6,
            _ => throw new ArgumentOutOfRangeException("Valid indexes are 0 through 5 inclusive.")
        };
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct GameTileItem {
        [FieldOffset(0x0)] public AddonWeeklyPuzzle* self;
        [FieldOffset(0x8)] public AtkComponentButton* Button;
        [FieldOffset(0x10)] public AtkResNode* RevealedIconResNode;
        [FieldOffset(0x18)] public AtkResNode* UnkRes20;
        [FieldOffset(0x20)] public AtkResNode* RevealedTileResNode;
        [FieldOffset(0x28)] public long Unk28;
    }
}
