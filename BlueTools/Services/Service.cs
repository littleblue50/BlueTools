using BlueTools.IPC;
using BlueTools.Utils;

namespace BlueTools.Services;

public static class Service
{
    public static class IPC
    {
        public static AutohookIPC AutohookIPC = new();
        public static LifestreamIPC LifestreamIPC = new();
        public static NavMeshIPC NavMeshIPC = new();
    }

    public static class Utils
    {
        public static Fishing Fishing = new();
        public static GameUI GameUI = new();
        public static PlayerHelper PlayerHelper = new();
        public static Shop Shop = new();
        public static Travel Travel = new();
        public static Weather Weather = new();
    }
    
    public static ModuleManager ModuleManager = null!;
    
}