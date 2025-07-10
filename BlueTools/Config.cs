using ECommons.Configuration;

namespace BlueTools;

public class Config : IEzConfig
{
    public int Version { get; set; } = 2;
    
    // Diadem Helper Configuration
    public int DiademTargetGrade { get; set; } = 4;
    public bool DiademShouldFish { get; set; } = true;
    public bool DiademShouldGather { get; set; } = false;
    public int DiademMinBaitCount { get; set; } = 0;
    public int DiademMaxBaitCount { get; set; } = 99;
}