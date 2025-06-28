using ECommons.EzIpcManager;

#nullable disable
namespace BlueTools.IPC;

public class LifestreamIPC
{
    public const string Name = "Lifestream";
   
    public LifestreamIPC() => EzIPC.Init(this, Name);

    [EzIPC] public readonly Func<uint, byte, bool> Teleport;
    [EzIPC] public readonly Func<bool> AethernetTeleportToFirmament;
    [EzIPC] public readonly Func<bool> IsBusy;
}