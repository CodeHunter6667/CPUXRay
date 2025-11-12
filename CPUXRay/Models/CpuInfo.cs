namespace CPUXRay.Models;

public class CpuInfo
{
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public int Cores { get; set; }
    public int LogicalProcessors { get; set; }
    public double MaxClockSpeed { get; set; } // MHz
    public double CurrentClockSpeed { get; set; }
    public string Socket { get; set; } = string.Empty;
    public string ProcessorId { get; set; } = string.Empty;
    public int L2CacheSize { get; set; } // KB
    public int L3CacheSize { get; set; }
    public double Temperature { get; set; }
    public double Usage { get; set; }
}
