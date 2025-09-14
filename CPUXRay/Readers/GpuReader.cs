using CPUXRay.Hardware;
using CPUXRay.Models;
using LibreHardwareMonitor.Hardware;
using System.Linq;

namespace CPUXRay.Readers;

public static class GpuReader
{
    public static GpuInfo GetGpuInfo()
    {
        var gpu = new GpuInfo();
        
        foreach (var hardware in SensorManager.GetHardware(HardwareType.GpuNvidia)
            .Concat(SensorManager.GetHardware(HardwareType.GpuAmd))
            .Concat(SensorManager.GetHardware(HardwareType.GpuIntel)))
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
        return gpu;
    }
}
