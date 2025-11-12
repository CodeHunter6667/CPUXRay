namespace CPUXRay.Models;

public class GpuInfo
{
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public long VideoMemory { get; set; } // Bytes
    public string DriverVersion { get; set; } = string.Empty;
    public string DriverDate { get; set; } = string.Empty;
    public int CurrentRefreshRate { get; set; }
    public string VideoProcessor { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Usage { get; set; }
    public int CoreClock { get; set; }
    public int MemoryClock { get; set; }
}
