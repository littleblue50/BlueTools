using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using FFXIVClientStructs.Havok.Common.Base.Math.Matrix;
using FFXIVClientStructs.Havok.Common.Base.Object;
using FFXIVClientStructs.Havok.Common.Base.Types;
using FFXIVClientStructs.Havok.Common.SceneData.Mesh;

namespace FFXIVClientStructs.Havok.Animation.Deform.Skinning;

[GenerateInterop]
[Inherits<hkReferencedObject>]
[StructLayout(LayoutKind.Explicit, Size = 0x50)]
public partial struct hkaMeshBinding {
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct Mapping {
        [FieldOffset(0x00)] public hkArray<short> _Mapping;
    }

    [FieldOffset(0x10)] public hkRefPtr<hkxMesh> Mesh;
    [FieldOffset(0x18)] public hkStringPtr OriginalSkeletonName;
    [FieldOffset(0x20)] public hkStringPtr Name;
    [FieldOffset(0x28)] public hkRefPtr<hkaSkeleton> Skeleton;
    [FieldOffset(0x30)] public hkArray<Mapping> Mappings;
    [FieldOffset(0x40)] public hkArray<hkTransformf> BoneFromSkinMeshTransforms;
}
