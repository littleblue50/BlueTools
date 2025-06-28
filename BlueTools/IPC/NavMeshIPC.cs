using ECommons.EzIpcManager;
using ECommons.Logging;
using System.Numerics;

#nullable disable
namespace BlueTools.IPC;

public class NavMeshIPC
{
    public const string Name = "vnavmesh";
   
    public NavMeshIPC() 
    {
        PluginLog.Information("NavMeshIPC constructor called");
        EzIPC.Init(this, Name);
        PluginLog.Information("NavMeshIPC constructor completed");
    }

    [EzIPC("Nav.%m")] public readonly Func<bool> IsReady;
    [EzIPC("Path.%m")] public readonly Func<bool> IsRunning;
    [EzIPC("Path.%m")] public readonly Action Stop;
    
    [EzIPC("SimpleMove.%m")] public readonly Func<bool> PathfindInProgress;
    [EzIPC("SimpleMove.%m")] public readonly Func<Vector3, bool, bool> PathfindAndMoveTo;
    [EzIPC("SimpleMove.%m")] public readonly Func<Vector3, bool, float, bool> PathfindAndMoveCloseTo;

    [EzIPC("Query.Mesh.%m")] public readonly Func<Vector3, float, float, Vector3?> NearestPoint;
    [EzIPC("Query.Mesh.%m")] public readonly Func<Vector3, bool, float, Vector3?> PointOnFloor;

}