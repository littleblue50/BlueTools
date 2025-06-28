using FFXIVClientStructs.Havok.Common.Base.Math.Vector;

namespace FFXIVClientStructs.Havok.Common.Base.Math.Quaternion;

[GenerateInterop]
[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public unsafe partial struct hkQuaternionf {
    [FieldOffset(0x00)] public float X;
    [FieldOffset(0x04)] public float Y;
    [FieldOffset(0x08)] public float Z;
    [FieldOffset(0x0C)] public float W;

    [MemberFunction("F3 0F 11 54 24 ?? 48 83 EC 38")]
    public partial void setAxisAngle1(hkVector4f* axis, float angle);

    [MemberFunction("E8 ?? ?? ?? ?? 44 0F 28 2D")]
    public partial void setAxisAngle2(hkVector4f* axis, hkSimdFloat32 angle);

    [MemberFunction("48 8B C4 F3 0F 11 58 ?? F3 0F 11 50 ?? F3 0F 11 48 ??")]
    public partial void setFromEulerAngles1(float roll, float pitch, float yaw);

    [MemberFunction("48 8B C4 48 81 EC ?? ?? ?? ?? 66 0F 6F 25")]
    public partial void setFromEulerAngles2(hkSimdFloat32* roll, hkSimdFloat32* pitch, hkSimdFloat32* yaw);

    [MemberFunction("E8 ?? ?? ?? ?? 0F 28 0C 1F")]
    public partial void setSlerp(hkQuaternionf* q0, hkQuaternionf* q1, hkSimdFloat32* t);

    [MemberFunction("48 83 EC 48 0F 28 1D")]
    public partial hkSimdFloat32 getAngleSr();

    [MemberFunction("0F 28 11 F3 0F 11 4C 24")]
    public partial byte isOk(float epsilon);
}
