using BlueTools.Enums;
using BlueTools.Services;
using BlueTools.Data;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.ExcelServices;
using BlueTools.Utils;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace BlueTools.Modules;

public record ShopNPCInfo(Vector3 Position, ulong VendorId, uint ShopId);

public unsafe class DiademHelper : BaseModule
{
    public override string Name => "Diadem Helper";

    private InventoryManager* inventoryManager => InventoryManager.Instance();
    private ShopNPCInfo merchantNPC = new(new Vector3(-641.9315f,285.2844f,-138.1888f), 4303540818, 263016);
    private ulong diademEntryNPCObjectId = 4303233947;
    
    private int currentTargetGrade = BlueTools.Config.DiademTargetGrade;

    private bool hitAfkCheck = false;
    
    private enum ActionState
    {
        Idle,
        TravellingToFirmament,
        EnteringDiadem,
        RestockingBait,
    }

    private ActionState actionState = ActionState.Idle;
    protected override void OnEnable()
    {
        PluginLog.Information($"{Name} module enabled");
        actionState = ActionState.Idle;

        Service.Utils.Shop.ClearSession(); // Clear any stuck shop states
    }
    
    protected override void OnDisable()
    {
        // Stop any ongoing pathfinding when disabled
        if (Service.IPC.NavMeshIPC.IsRunning())
        {
            Service.IPC.NavMeshIPC.Stop();
        }
        
        if (P.TaskManager.IsBusy)
        {
            P.TaskManager.Abort();
        }

        StopFishing();
        
        // Clear shop states to prevent stuck states on restart
        Service.Utils.Shop.ClearSession();
        
        // Clear fishing position cache to ensure fresh random positions on next enable
        DiademFish.ClearPositionCache();

        actionState = ActionState.Idle;
    }
    
    public override void Tick()
    {
        if (Service.ModuleManager.ActiveModule != this) return;
        if (!Player.Available) return;
        if (Player.Territory != (uint)TerritoryType.Diadem)
        {
            EnterDiadem();
            return;
        }
        
        // Check if target grade changed and abort current tasks if needed
        if (currentTargetGrade != BlueTools.Config.DiademTargetGrade)
        {
            PluginLog.Information($"Target grade changed from {currentTargetGrade} to {BlueTools.Config.DiademTargetGrade}, aborting current tasks");
            
            // Stop any navigation
            if (Service.IPC.NavMeshIPC.IsRunning())
            {
                Service.IPC.NavMeshIPC.Stop();
            }
            
            // Abort TaskManager tasks
            P.TaskManager.Abort();
            
            // Update to new target grade
            currentTargetGrade = BlueTools.Config.DiademTargetGrade;
        }
        
        if (!EzThrottler.Throttle("DiademHelperTick", 1000)) return;
        
        if (BlueTools.Config.DiademShouldFish)
        {
            Fish();
        }
        
        if (BlueTools.Config.DiademShouldGather)
        {
            Gather();
        }
    } 

    private void EnterDiadem()
    {
        if (Player.IsBusy || Player.IsInDuty) return;
        if (P.TaskManager.IsBusy) return;
        if (Service.IPC.NavMeshIPC.IsRunning()) return;
        if (Service.IPC.LifestreamIPC.IsBusy()) return;
        
        // Don't try to enter if we're already in a duty queue or transitioning
        if (Svc.Condition[ConditionFlag.BoundByDuty] || 
            Svc.Condition[ConditionFlag.WaitingForDuty] ||
            Svc.Condition[ConditionFlag.WaitingForDutyFinder]) return;
        
        if (Player.Job != Job.BTN && Player.Job != Job.FSH && Player.Job != Job.MIN)
        {
            bool changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.BTN);
            if (!changedJob)
            {
                changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.MIN);
            }
            if (!changedJob)
            {
                changedJob = Service.Utils.PlayerHelper.ChangeJob(Job.FSH);
            }
            
            if (!changedJob)
            {
                PluginLog.Error("Failed to become a gatherer, disabling module");
                Disable();
                return;
            }
        }

        if (Player.Territory == (uint)TerritoryType.Firmament)
        {
            if (actionState == ActionState.EnteringDiadem) return;
            actionState = ActionState.EnteringDiadem;

            PluginLog.Information("In Firmament, queueing for Diadem");
            P.TaskManager.Enqueue(() => Travel.MoveToPosition(new Vector3(-19.187387f, -16f, 142.10204f)));
            
            var npc = Svc.Objects.First(x => x.GameObjectId == diademEntryNPCObjectId);
            P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => npc));
            
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SkipTalk());
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SelectListIndex(0));
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.SelectYesno());
            P.TaskManager.Enqueue(() => Service.Utils.GameUI.ConfirmContentsFinder());

            actionState = ActionState.Idle;
        }
        else
        {
            if (actionState == ActionState.TravellingToFirmament) return;
            actionState = ActionState.TravellingToFirmament;

            PluginLog.Information("Teleporting to Foundation and then Firmament to enter Diadem");
            P.TaskManager.Enqueue(() => Service.IPC.LifestreamIPC.Teleport((uint)Aetheryte.Foundation, 0));
            P.TaskManager.Enqueue(() => Player.Territory == (uint)TerritoryType.Foundation && !Player.IsBusy);
            P.TaskManager.Enqueue(() => Service.IPC.LifestreamIPC.AethernetTeleportToFirmament());
        }
    }

    private void Gather()
    {
        // stub for now until implemented
    }
    
    private void Fish()
    {
        if (P.TaskManager.IsBusy) return;

        if (Player.Job != Job.FSH)
        {
            if (!Service.Utils.PlayerHelper.ChangeJob(Job.FSH))
            {
                PluginLog.Error("Failed to become a fisher, disabling module");
                Disable();
                return;
            }
        }
        
        var currentWeather = Weather.GetCurrentWeather();
        var targetGrade = BlueTools.Config.DiademTargetGrade;

        if (!currentWeather.HasValue) return;

        var fishingPosition = DiademFish.GetFishingPosition(targetGrade, currentWeather.Value);
        var autohookPreset = DiademFish.GetAutohookPreset(targetGrade, currentWeather.Value);
        var requiredBait = DiademFish.GetBait(targetGrade, currentWeather.Value);
        
        if (fishingPosition == null || autohookPreset == null)
        {
            return;
        }

        // Check if we need to restock bait first
        if (NeedToRestockBait())
        {
            PluginLog.Information("Low bait detected, restocking at merchant");
            RestockBait();
            return;
        }

        // Check if we're in the right position
        if (!IsAtCorrectFishingPosition(targetGrade, currentWeather.Value))
        {
            StopFishing();
            MoveToFishingPosition(fishingPosition, targetGrade, currentWeather.Value);
            return;
        }

        // Check if we have the right bait
        var equippedBait = (BaitType?)PlayerState.Instance()->FishingBait;
        if (requiredBait.HasValue && requiredBait.Value != equippedBait)
        {
            if (Svc.Condition[ConditionFlag.Fishing])
            {
                StopFishing();
                return;
            }
            
            if (EzThrottler.Throttle("BaitChangeAttempt", 2000))
            {
                PluginLog.Information($"Changing bait to {requiredBait.Value}");
                Fishing.ChangeBait(requiredBait.Value);
            }
            return;
        }

        // Start fishing if not already fishing and ready
        var isFishing = Svc.Condition[ConditionFlag.Fishing] || Svc.Condition[ConditionFlag.Gathering];
        var navMeshRunning = Service.IPC.NavMeshIPC.IsRunning();
        var pathfindInProgress = Service.IPC.NavMeshIPC.PathfindInProgress();
        
        if (EzThrottler.Throttle("AntiAfkCheck", 1000))
        {
            CheckForFishAmissMessage();
        }
        
        if (!isFishing && !Player.Mounted && !navMeshRunning && !pathfindInProgress && !hitAfkCheck)
        {
            StartFishing(autohookPreset);
        }
        else if (hitAfkCheck)
        {
            RotateToNextFishingPosition();
        }
    }

    private bool NeedToRestockBait()
    {
        var targetGrade = BlueTools.Config.DiademTargetGrade;
        var requiredBaits = GetRequiredBaitsForGrade(targetGrade);
        
        // Check if we have at least one of each possible bait type for this grade
        foreach (var bait in requiredBaits)
        {
            var currentCount = inventoryManager->GetInventoryItemCount((uint)bait);
            if (currentCount <= BlueTools.Config.DiademMinBaitCount)
            {
                PluginLog.Information($"Need to restock bait {bait}");
                return true;
            }
        }
        return false;
    }

    private HashSet<BaitType> GetRequiredBaitsForGrade(int grade)
    {
        var baits = new HashSet<BaitType>();
        
        // Get all possible baits for this grade across all weather conditions
        var weatherTypes = new[] 
        { 
            WeatherType.Snow, 
            WeatherType.UmbralDuststorms, 
            WeatherType.UmbralFlare, 
            WeatherType.UmbralLevin, 
            WeatherType.UmbralTempest 
        };
        
        foreach (var weather in weatherTypes)
        {
            var bait = DiademFish.GetBait(grade, weather);
            if (bait.HasValue)
            {
                baits.Add(bait.Value);
            }
        }
        
        return baits;
    }

    private bool IsAtCorrectFishingPosition(int targetGrade, WeatherType currentWeather)
    {
        if (Player.Mounted) return false;

        var spotInfo = DiademFish.GetFishingSpot(targetGrade, currentWeather);
        if (spotInfo == null) return false;

        foreach (var pos in spotInfo.FishingPositions)
        {
            if (Player.DistanceTo(pos) <= 1.0f)
            {
                return true;
            }
        }
        return false;
    }

    private void MoveToFishingPosition(Vector3? fishingPosition, int targetGrade, WeatherType currentWeather)
    {
        if (Service.IPC.NavMeshIPC.IsRunning()) return;
        if (P.TaskManager.IsBusy) return;
        
        PluginLog.Information($"Moving to fishing spot for {currentWeather} Grade {targetGrade}");
            
        if (fishingPosition.HasValue)
        {
            var landingPosition = DiademFish.GetLandingPosition(targetGrade, currentWeather);
            
            if (landingPosition.HasValue)
            {
                var landingPos = landingPosition.Value;
                var fishingPos = fishingPosition.Value;
                var distanceToLanding = Player.DistanceTo(landingPos);
                
                // Only enqueue landing movement if we're not already there
                if (distanceToLanding > 1.0f)
                {
                    P.TaskManager.Enqueue(() => Travel.MoveToPosition(landingPos, false), "MoveLanding");
                }
                
                P.TaskManager.Enqueue(() => Travel.MoveToPosition(fishingPos, true), "MoveFishing");
            }
            else
            {
                var fishingPos = fishingPosition.Value;
                P.TaskManager.Enqueue(() => Travel.MoveToPosition(fishingPos), "MoveFishingDirect");
            }
        }
    }

    private void StartFishing(string autohookPreset)
    {
        if (P.TaskManager.IsBusy) return;
        
        // delete all anonymous presets before creating one to be safe
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.DeleteAllAnonymousPresets());
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.CreateAndSelectAnonymousPreset(autohookPreset));
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.SetPluginState(true));
        P.TaskManager.Enqueue(() => Fishing.StartFishing());
    }

    private void StopFishing()
    {
        if (P.TaskManager.IsBusy) return;
        if (!Svc.Condition[ConditionFlag.Fishing]) return;

        P.TaskManager.Enqueue(() => Fishing.QuitFishing());
        P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.DeleteAllAnonymousPresets());
    }

    private void CheckForFishAmissMessage()
    {
        PluginLog.Debug("CheckForFishAmissMessage: Checking for _ScreenText addon");
        
        if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("_ScreenText", out var addon) && addon->IsVisible)
        {
            PluginLog.Debug("CheckForFishAmissMessage: _ScreenText addon found and visible");
            
            var componentNode = GenericHelpers.GetNodeByIDChain(addon->RootNode, 1, 40001);
            var node = GenericHelpers.GetNodeByIDChain(addon->RootNode, 1, 40001, 2, 3);
            if (componentNode->IsVisible() && node != null && node->GetNodeType() == NodeType.Text)
            {
                var textNode = node->GetAsAtkTextNode();
                var nodeText = textNode->NodeText.ToString();

                PluginLog.Information($"CheckForFishAmissMessage: Detected message: '{nodeText}'");

                // Check if the message contains the "fish sense something amiss" text
                if (nodeText.Contains("fish sense something amiss", StringComparison.OrdinalIgnoreCase) ||
                    nodeText.Contains("fish here have grown wise", StringComparison.OrdinalIgnoreCase))
                {
                    PluginLog.Information($"Anti-AFK message detected! Message: '{nodeText}'");
                    hitAfkCheck = true;
                }
                else
                {
                    hitAfkCheck = false;
                    PluginLog.Debug($"CheckForFishAmissMessage: Message does not match fish amiss patterns: '{nodeText}'");
                }
            }
            else
            {
                PluginLog.Debug("CheckForFishAmissMessage: No text node found or wrong node type: " + node->GetNodeType());
            }
        }
        else
        {
            PluginLog.Debug("CheckForFishAmissMessage: _ScreenText addon not found or not visible");
        }
    }


    /// <summary>
    /// Call this when you get the "fish sense something amiss" message to rotate to the next fishing spot
    /// </summary>
    public void RotateToNextFishingPosition()
    {
        if (P.TaskManager.IsBusy) return;
        
        var currentWeather = Weather.GetCurrentWeather();
        var targetGrade = BlueTools.Config.DiademTargetGrade;
        
        // We need a valid weather type for Diadem fishing
        if (!currentWeather.HasValue) return;
        
        PluginLog.Information("Fish sense something amiss - rotating to next fishing position");
        
        StopFishing();
        
        // Get the next fishing position and move there
        var nextPosition = DiademFish.GetNextFishingPosition(targetGrade, currentWeather.Value);
        MoveToFishingPosition(nextPosition, targetGrade, currentWeather.Value);
        
        hitAfkCheck = false;

        var autohookPreset = DiademFish.GetAutohookPreset(targetGrade, currentWeather.Value);
        if (autohookPreset != null)
        {
            P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.DeleteAllAnonymousPresets());
            P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.CreateAndSelectAnonymousPreset(autohookPreset));
            P.TaskManager.Enqueue(() => Service.IPC.AutohookIPC.SetPluginState(true));
            P.TaskManager.Enqueue(() => Fishing.StartFishing());
        }
    }

    private void RestockBait()
    {
        if (P.TaskManager.IsBusy) return;

        var targetGrade = BlueTools.Config.DiademTargetGrade;
        var requiredBaits = GetRequiredBaitsForGrade(targetGrade);
        
        var itemsToBuy = new List<(uint itemId, int count)>();
        
        foreach (var bait in requiredBaits)
        {
            var currentCount = inventoryManager->GetInventoryItemCount((uint)bait);
            if (currentCount < BlueTools.Config.DiademMaxBaitCount)
            {
                itemsToBuy.Add(((uint)bait, BlueTools.Config.DiademMaxBaitCount - currentCount));
            }
        }
        
        // Only proceed if there are items to buy
        if (itemsToBuy.Count == 0) return;
        
        PluginLog.Information($"Restocking bait for Grade {targetGrade}");
        
        // Move to merchant first
        P.TaskManager.Enqueue(() => Travel.MoveToPosition(merchantNPC.Position));
        
        // Buy all required baits
        P.TaskManager.Enqueue(() => Service.Utils.Shop.BuyFromShop(merchantNPC.VendorId, merchantNPC.ShopId, itemsToBuy.ToArray()));
    }

} 