using ECommons.EzIpcManager;

#nullable disable
namespace BlueTools.IPC;

public class AutohookIPC
{
    public const string Name = "AutoHook";
   
    public AutohookIPC() => EzIPC.Init(this, Name);

    [EzIPC] public readonly Action<bool> SetPluginState;
    [EzIPC] public readonly Action<string> CreateAndSelectAnonymousPreset;
    [EzIPC] public readonly Action DeleteAllAnonymousPresets;
}