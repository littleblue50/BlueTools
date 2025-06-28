namespace FFXIVClientStructs.FFXIV.Client.System.Threading;

// Client::System::Threading::Thread
//   Client::System::Common::NonCopyable
[GenerateInterop(isInherited: true)]
[StructLayout(LayoutKind.Explicit, Size = 0x28)]
public unsafe partial struct Thread {
    [FieldOffset(0x00)] public byte** args;
    [FieldOffset(0x08)] public nint EventHandle;
    [FieldOffset(0x10)] public nint ThreadHandle;
    [FieldOffset(0x18)] public int ThreadId;
    [FieldOffset(0x1C)] public int AffinityMask;
}
