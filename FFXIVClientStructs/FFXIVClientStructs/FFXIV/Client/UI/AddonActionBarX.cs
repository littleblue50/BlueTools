using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonActionBarX
//   Client::UI::AddonActionBarBase
//     Component::GUI::AtkUnitBase
//       Component::GUI::AtkEventListener
[Addon("_ActionBar01", "_ActionBar02", "_ActionBar03", "_ActionBar04", "_ActionBar05", "_ActionBar06", "_ActionBar07", "_ActionBar08", "_ActionBar09")]
[GenerateInterop(isInherited: true)]
[Inherits<AddonActionBarBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x2B0)]
public unsafe partial struct AddonActionBarX {
    [FieldOffset(0x260)] public AtkTextNode* HotbarNumIconTextNode;
    [FieldOffset(0x268)] public AtkCollisionNode* HotbarNumIconCollisionNode;
    [FieldOffset(0x270)] public AtkResNode* ContainerNode;
    [FieldOffset(0x278)] public AtkResNode* HotbarNumIconNode;
    [FieldOffset(0x280)] public AtkResNode* PadlockNode;

    /// <summary>
    /// The current layout (columns x rows) of this specific hotbar.
    /// </summary>
    [FieldOffset(0x288)] public ActionBarLayout ActionBarLayout;

    [FieldOffset(0x294), FixedSizeArray] internal FixedSizeArray6<Dimensions> _layoutDimensions; // every hotbar stores the same pre-baked dimensions for each of the 6 layout options

    [StructLayout(LayoutKind.Explicit, Size = 0x4)]
    public struct Dimensions {
        [FieldOffset(0x0)] public short Width;
        [FieldOffset(0x2)] public short Height;
    }
}

public enum ActionBarLayout : byte {
    Layout12X1 = 0,
    Layout6X2 = 1,
    Layout4X3 = 2,
    Layout3X4 = 3,
    Layout2X6 = 4,
    Layout1X12 = 5
}
