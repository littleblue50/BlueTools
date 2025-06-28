using BlueTools.Services;

namespace BlueTools.Modules;

public abstract class BaseModule
{
    /// <summary>
    /// The display name of this module
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// Whether this module is currently active
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Enable this module and set it as the active module in ModuleManager
    /// </summary>
    public virtual void Enable()
    {
        if (IsActive) return;
        
        try
        {
            OnEnable();
            IsActive = true;
            Service.ModuleManager.SetActiveModule(this);
        }
        catch (Exception e)
        {
            IsActive = false;
            PluginLog.Error($"Failed to enable module {Name}: {e}");
        }
    }
    
    /// <summary>
    /// Disable this module and clear it from ModuleManager if it's the active one
    /// </summary>
    public virtual void Disable()
    {
        if (!IsActive) return;
        
        try
        {
            OnDisable();
        }
        finally
        {
            IsActive = false;
            Services.Service.ModuleManager.ClearActiveModule(this);
        }
    }
    
    /// <summary>
    /// Override this method to implement module-specific enable logic
    /// </summary>
    protected virtual void OnEnable() { }
    
    /// <summary>
    /// Override this method to implement module-specific disable logic
    /// </summary>
    protected virtual void OnDisable() { }
    
    /// <summary>
    /// Override this method to implement the main module logic that runs when active
    /// </summary>
    public abstract void Tick();
} 