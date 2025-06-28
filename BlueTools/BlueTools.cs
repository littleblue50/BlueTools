using BlueTools.Services;
using BlueTools.UI;
using BlueTools.Modules;
using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.SimpleGui;
using ECommons.Singletons;

namespace BlueTools;

public sealed class BlueTools : IDalamudPlugin
{
    internal static Config Config { get; private set; } = null!;
    internal static BlueTools P { get; private set; } = null!;

    public TaskManager TaskManager { get; private set; }

    public BlueTools(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        
        ECommonsMain.Init(pluginInterface, this, Module.DalamudReflector, Module.ObjectFunctions);
        EzConfigGui.Init(new MainWindow());

        Config = EzConfig.Init<Config>();
        EzConfig.Save();
        
        SingletonServiceManager.Initialize(typeof(Service));

        PluginLog.Information("BlueTools initialized");
        TaskManager = new(new(showDebug: true));
        
        Service.ModuleManager.RegisterModule<DiademHelper>();
        
        Svc.Framework.Update += OnFrameworkUpdate;
    }
    
    private void OnFrameworkUpdate(object framework)
    {
        Service.ModuleManager.Tick();
    }
    
    public void Dispose()
    {
        Svc.Framework.Update -= OnFrameworkUpdate;
        Service.ModuleManager.Dispose();
        ECommonsMain.Dispose();
    }
}
