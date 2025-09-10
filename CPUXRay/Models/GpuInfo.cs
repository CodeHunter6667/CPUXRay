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

    public static GpuInfo GetGpuInfo()
    {
        var gpu = new GpuInfo();
        var computer = new Computer
        {
            IsGpuEnabled = true
        };

        computer.Open();

        foreach (var hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel)
            {
                hardware.Update();
                gpu.Name = hardware.Name;
                gpu.Manufacturer = hardware.HardwareType.ToString().Replace("Gpu", "");

                foreach (var sensor in hardware.Sensors)
                {
                    switch (sensor.SensorType)
                    {
                        case SensorType.Temperature when sensor.Name.Contains("GPU Core"):
                            gpu.TemperatureCelsius = sensor.Value ?? 0;
                            break;

                        case SensorType.Load when sensor.Name.Contains("GPU Core"):
                            gpu.UsagePercentage = sensor.Value ?? 0;
                            break;

                        case SensorType.Clock when sensor.Name.Contains("GPU Core"):
                            gpu.CoreClockMHz = sensor.Value ?? 0;
                            break;

                        case SensorType.Clock when sensor.Name.Contains("GPU Memory"):
                            gpu.MemoryClockMHz = sensor.Value ?? 0;
                            break;

                        case SensorType.SmallData when sensor.Name.Contains("GPU Memory Total"):
                            gpu.TotalMemoryMB = (ulong)(sensor.Value ?? 0);
                            break;

                        case SensorType.SmallData when sensor.Name.Contains("GPU Memory Used"):
                            gpu.UsedMemoryMB = (ulong)(sensor.Value ?? 0);
                            break;
                    }
                }
            }
        }
        computer.Close();
        return gpu;
    }
}
