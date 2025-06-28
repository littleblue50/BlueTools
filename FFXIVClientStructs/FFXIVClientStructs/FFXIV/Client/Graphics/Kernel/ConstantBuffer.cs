namespace FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;

// Client::Graphics::Kernel::ConstantBuffer
//   Client::Graphics::Kernel::Buffer
//     Client::Graphics::Kernel::Resource
//       Client::Graphics::DelayedReleaseClassBase
//         Client::Graphics::ReferencedClassBase
/// <summary>
/// A constant buffer (or cbuffer), which is used to send values to shaders. Usually contains 4*n floats.
/// </summary>
[GenerateInterop]
[StructLayout(LayoutKind.Explicit, Size = 0x70)]
public unsafe partial struct ConstantBuffer {
    public const byte DefaultLoadSourcePointerFlags = 2;

    [FieldOffset(0x20)]
    public int ByteSize;
    [FieldOffset(0x24)]
    public int Flags;
    [FieldOffset(0x28)]
    public void* UnsafeSourcePointer;

    public void* TryGetSourcePointer()
        => (Flags & 0x4003) == 0 ? UnsafeSourcePointer : null;

    public Span<float> TryGetBuffer()
        => TryGetBuffer<float>();

    public Span<TContents> TryGetBuffer<TContents>() where TContents : unmanaged {
        var sourcePointer = TryGetSourcePointer();
        if (sourcePointer != null)
            return new Span<TContents>(sourcePointer, ByteSize / sizeof(TContents));
        else
            return default;
    }

    public Span<TContents> LoadBuffer<TContents>(int offset, int length, byte flags = DefaultLoadSourcePointerFlags) where TContents : unmanaged {
        var sourcePointer = LoadSourcePointer(offset * sizeof(TContents), length * sizeof(TContents), flags);
        if (sourcePointer != null)
            return new Span<TContents>(sourcePointer, length);
        else
            return default;
    }

    [MemberFunction("E8 ?? ?? ?? ?? 88 5C 24 40")]
    public partial void* LoadSourcePointer(int byteOffset, int byteSize, byte flags = DefaultLoadSourcePointerFlags);
}

/// <summary>
/// A helper to manipulate a constant buffer that holds contents of a known type.
/// </summary>
/// <typeparam name="TContents">Type of the cbuffer's contents. Usually a container of floats or vectors thereof.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ConstantBufferPointer<TContents> where TContents : unmanaged {
    public ConstantBuffer* CBuffer;

    public int Length
        => CBuffer == null ? 0 : (CBuffer->ByteSize / sizeof(TContents));

    public ConstantBufferPointer(ConstantBuffer* cBuffer) {
        CBuffer = cBuffer;
    }

    public Span<TContents> TryGetBuffer()
        => CBuffer == null ? CBuffer->TryGetBuffer<TContents>() : default;

    public Span<TContents> LoadBuffer(int offset, int length, byte flags = ConstantBuffer.DefaultLoadSourcePointerFlags)
        => CBuffer == null ? CBuffer->LoadBuffer<TContents>(offset, length, flags) : default;
}
