using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonActionDoubleCrossBase
//   Client::UI::AddonActionBarBase
//     Component::GUI::AtkUnitBase
//       Component::GUI::AtkEventListener
[Addon("_ActionDoubleCrossL", "_ActionDoubleCrossR")]
[GenerateInterop]
[Inherits<AddonActionBarBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x310)]
public unsafe partial struct AddonActionDoubleCrossBase {
    [FieldOffset(0x260)] public AtkResNode* ContainerNode;
    [FieldOffset(0x268)] public AtkComponentNode* SlotContainerL;
    [FieldOffset(0x270)] public AtkComponentNode* SlotContainerR;

    [FieldOffset(0x278), FixedSizeArray] internal FixedSizeArray4<AddonActionCross.HelpMessage> _hotbarHelpL;

    [FieldOffset(0x2B8), FixedSizeArray] internal FixedSizeArray4<AddonActionCross.HelpMessage> _hotbarHelpR;

    /// <summary>
    /// Indicates whether this bar is selected.
    /// </summary>
    [FieldOffset(0x2F8)] public bool Selected;

    /// <summary>
    /// Set to 1 when the WXHB is showing the directional pad inputs as well as the action button inputs.
    /// </summary>
    [FieldOffset(0x2F9)] public byte ShowDPadSlots;

    [FieldOffset(0x2FA)] public bool AlwaysDisplay;
    [FieldOffset(0x2FB)] public bool OtherBarSelected; // true if any bar other than this one is selected
    [FieldOffset(0x2FC)] public bool DoubleTapDetected; // true if the currently-held trigger matches the previous trigger press.

    /// <summary>
    /// The ID of the hotbar in <see cref="RaptureHotbarModule"/> that this action bar is currently referencing.
    /// </summary>
    [FieldOffset(0x300)] public byte BarTarget;

    /// <summary>
    /// Use the left side (slots 0-7) of the hotbar specified in <see cref="BarTarget"/>.
    /// </summary>
    /// <remarks>
    /// In effect, when this is false, it means any given HotBarSlot will be at index +8 from the index in the UI.
    /// </remarks>
    [FieldOffset(0x304)] public byte UseLeftSide;

    /// <summary>
    /// Set to 1 when the WXHB is in "merged positioning" mode with the normal cross hotbar.
    /// </summary>
    [FieldOffset(0x305)] public byte MergedPositioning;
}
