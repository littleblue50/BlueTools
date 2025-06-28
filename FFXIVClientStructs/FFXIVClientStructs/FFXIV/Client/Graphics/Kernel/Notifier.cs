namespace FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;

// Client::Graphics::Kernel::Notifier
[GenerateInterop(isInherited: true)]
[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public unsafe partial struct Notifier {
    [FieldOffset(0x08)] public Notifier* Next;
    [FieldOffset(0x10)] public Notifier* Prev;
}
