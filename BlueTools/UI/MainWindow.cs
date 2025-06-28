using BlueTools.Services;
using BlueTools.Modules;
using ECommons.SimpleGui;
using ECommons.Configuration;
using ImGuiNET;

namespace BlueTools.UI;

public class MainWindow : ConfigWindow
{
    public override void Draw()
    {
        string weatherText = Utils.Weather.GetWeatherName();
        ImGuiEx.Text($"Current Weather: {weatherText}");
        
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        
        // Diadem Helper Module Control
        var diademHelper = Service.ModuleManager.GetModule<DiademHelper>();
        if (diademHelper != null)
        {
            // Target Grade Selection
            ImGuiEx.Text("Target Grade:");
            ImGui.SameLine();
            
            var currentGrade = BlueTools.Config.DiademTargetGrade;
            if (ImGui.RadioButton("Grade 2", currentGrade == 2))
            {
                BlueTools.Config.DiademTargetGrade = 2;
                EzConfig.Save();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Grade 3", currentGrade == 3))
            {
                BlueTools.Config.DiademTargetGrade = 3;
                EzConfig.Save();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Grade 4", currentGrade == 4))
            {
                BlueTools.Config.DiademTargetGrade = 4;
                EzConfig.Save();
            }
            
            ImGui.Spacing();
            
            // Activity Selection Checkboxes
            ImGuiEx.Text("Activities:");
            
            var shouldFish = BlueTools.Config.DiademShouldFish;
            if (ImGui.Checkbox("Should Fish", ref shouldFish))
            {
                BlueTools.Config.DiademShouldFish = shouldFish;
                EzConfig.Save();
            }
            
            var shouldGather = BlueTools.Config.DiademShouldGather;
            if (ImGui.Checkbox("Should Gather", ref shouldGather))
            {
                BlueTools.Config.DiademShouldGather = shouldGather;
                EzConfig.Save();
            }
            
            ImGui.Spacing();
            
            // Bait Count Configuration
            ImGuiEx.Text("Bait Count:");
            ImGui.SameLine();
            
            var baitCount = BlueTools.Config.DiademBaitCount;
            ImGui.SetNextItemWidth(100);
            if (ImGui.SliderInt("##BaitCount", ref baitCount, 1, 999))
            {
                BlueTools.Config.DiademBaitCount = baitCount;
                EzConfig.Save();
            }
            
            ImGui.Spacing();
            
            // Check if requirements are met before allowing module activation
            var canEnable = Service.IPC.NavMeshIPC.PathfindAndMoveTo != null && 
                           Service.IPC.AutohookIPC.SetPluginState != null;
            
            if (!canEnable && !diademHelper.IsActive)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // Gray
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new System.Numerics.Vector4(0.5f, 0.5f, 0.5f, 1.0f));
                ImGuiEx.Button("Start Diadem Helper (Missing Plugins)");
                ImGui.PopStyleColor(3);
                
                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.6f, 0.0f, 1.0f)); // Orange
                ImGuiEx.Text("⚠ Requires: vnavmesh and AutoHook plugins");
                ImGui.PopStyleColor();
            }
            else
            {
                var buttonText = diademHelper.IsActive ? "Stop Diadem Helper" : "Start Diadem Helper";
                var buttonColor = diademHelper.IsActive ? 
                    new System.Numerics.Vector4(0.8f, 0.2f, 0.2f, 1.0f) :  // Red for stop
                    new System.Numerics.Vector4(0.2f, 0.8f, 0.2f, 1.0f);   // Green for start
                
                ImGui.PushStyleColor(ImGuiCol.Button, buttonColor);
                if (ImGuiEx.Button(buttonText))
                {
                    if (diademHelper.IsActive)
                    {
                        diademHelper.Disable();
                        PluginLog.Information("Diadem Helper stopped via UI");
                    }
                    else
                    {
                        diademHelper.Enable();
                        PluginLog.Information("Diadem Helper started via UI");
                    }
                }
                ImGui.PopStyleColor();
            }
            
            // Status display
            var statusText = diademHelper.IsActive ? "Running" : "Stopped";
            var statusColor = diademHelper.IsActive ? 
                new System.Numerics.Vector4(0.2f, 0.8f, 0.2f, 1.0f) :  // Green
                new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 1.0f);   // Gray
            
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, statusColor);
            ImGuiEx.Text($"Status: {statusText}");
            ImGui.PopStyleColor();
            
            // Show target info when active
            // if (diademHelper.IsActive)
            // {
            //     ImGuiEx.Text($"Target: Grade {BlueTools.Config.DiademTargetGrade} fish");
            // }
        }
        else
        {
            ImGuiEx.Text("Diadem Helper module not found!");
        }
    }
}
