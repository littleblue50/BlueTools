using BlueTools.Modules;

namespace BlueTools.Services;

public unsafe class ModuleManager
{
    private readonly Dictionary<Type, BaseModule> registeredModules = new();
    
    /// <summary>
    /// The currently active module, or null if none is active
    /// </summary>
    internal BaseModule? ActiveModule;
     
    /// <summary>
    /// Register a module for use
    /// </summary>
    public T RegisterModule<T>() where T : BaseModule, new()
    {
        var moduleType = typeof(T);
        if (registeredModules.ContainsKey(moduleType))
        {
            return (T)registeredModules[moduleType];
        }
        
        var module = new T();
        registeredModules[moduleType] = module;
        PluginLog.Information($"Registered module: {module.Name}");
        return module;
    }
    
    /// <summary>
    /// Get a registered module
    /// </summary>
    public T? GetModule<T>() where T : BaseModule
    {
        return registeredModules.TryGetValue(typeof(T), out var module) ? (T)module : null;
    }
    
    /// <summary>
    /// Enable a specific module by type
    /// </summary>
    public void EnableModule<T>() where T : BaseModule
    {
        var module = GetModule<T>();
        module?.Enable();
    }
    
    /// <summary>
    /// Disable a specific module by type
    /// </summary>
    public void DisableModule<T>() where T : BaseModule
    {
        var module = GetModule<T>();
        module?.Disable();
    }
    
    /// <summary>
    /// Set the active module. This will disable any currently active module first.
    /// </summary>
    /// <param name="module">The module to set as active</param>
    public void SetActiveModule(BaseModule module)
    {
        // If there's already an active module that's different, disable it first
        if (ActiveModule != null && ActiveModule != module)
        {
            ActiveModule.Disable();
        }
        
        ActiveModule = module;
    }
    
    /// <summary>
    /// Clear the active module if it matches the provided module
    /// </summary>
    /// <param name="module">The module to clear (for safety)</param>
    public void ClearActiveModule(BaseModule module)
    {
        if (ActiveModule == module)
        {
            ActiveModule = null;
        }
    }
    
    /// <summary>
    /// Force clear any active module (use with caution)
    /// </summary>
    public void ClearActiveModule()
    {
        ActiveModule?.Disable();
        ActiveModule = null;
    }
    
    /// <summary>
    /// Ticks active module if there is one
    /// </summary>
    public void Tick()
    {
        try
        {
            ActiveModule?.Tick();
        }
        catch (Exception ex)
        {
            // Log error and disable the problematic module
            var faultyModule = ActiveModule;
            ActiveModule = null;
            faultyModule?.Disable();
            PluginLog.Error($"Module {faultyModule?.Name} encountered an error and was disabled: {ex}");
        }
    }
    
    /// <summary>
    /// Dispose all modules
    /// </summary>
    public void Dispose()
    {
        ClearActiveModule();
        foreach (var module in registeredModules.Values)
        {
            try
            {
                if (module.IsActive)
                    module.Disable();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error disposing module {module.Name}: {ex}");
            }
        }
        registeredModules.Clear();
    }
}