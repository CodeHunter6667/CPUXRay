using LibreHardwareMonitor.Hardware;

namespace CPUXRay.Models;

public class GpuInfo
{
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public float CoreClockMHz { get; set; }
    public float MemoryClockMHz { get; set; }
    public float TemperatureCelsius { get; set; }
    public float UsagePercentage { get; set; }
    public ulong TotalMemoryMB { get; set; }
    public ulong UsedMemoryMB { get; set; }
    public string DriverVersion { get; set; } = string.Empty;
}
