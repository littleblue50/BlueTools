using FFXIVClientStructs.FFXIV.Client.System.String;

namespace FFXIVClientStructs.FFXIV.Client.System.Input;

// Client::System::Input::ClipBoardInterface
[GenerateInterop(isInherited: true)]
[StructLayout(LayoutKind.Explicit, Size = 0x8)]
public unsafe partial struct ClipBoardInterface {
    /// <summary>
    /// Writes to the system clipboard.<br />
    /// Currently, <c>this</c> is unused; this might as well be a static function, but all uses of it are indirect.
    /// </summary>
    /// <param name="stringToCopy">[in] The string to copy with payloads.</param>
    /// <param name="copiedStringWithoutPayload">[inout] The copied string without payloads.</param>
    [VirtualFunction(1)]
    public partial void WriteToSystemClipboard(Utf8String* stringToCopy, Utf8String* copiedStringWithoutPayload);

    /// <summary>
    /// Retrieves the current system clipboard text.<br />
    /// Currently, <see cref="ClipBoard.SystemClipboardText"/> is updated after this call.
    /// </summary>
    /// <returns>The text retrieved from the system clipboard.</returns>
    [VirtualFunction(2)]
    public partial Utf8String* GetSystemClipboardText();

    /// <summary>
    /// Sets the copy staging text.<br />
    /// Currently, this effectively copies <paramref name="utf8String"/> to <see cref="ClipBoard.CopyStagingText"/>.
    /// </summary>
    /// <param name="utf8String">[in] The text.</param>
    [VirtualFunction(3)]
    public partial void SetCopyStagingText(Utf8String* utf8String);

    /// <summary>
    /// Sets the copy staging text to the system clipboard.<br />
    /// Currently, this effectively calls <see cref="WriteToSystemClipboard"/>, using <see cref="ClipBoard.CopyStagingText"/> as both parameters.
    /// </summary>
    [VirtualFunction(4)]
    public partial void ApplyCopyStagingText();
}
