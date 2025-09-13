using CPUXRay.Models;
using LibreHardwareMonitor.Hardware;
using System;
using System.Management;

namespace CPUXRay.Readers;

public static class CpuReader
{
    private static CpuInfo GetCpuStaticInfo()
    {
        var cpu = new CpuInfo();

        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        foreach (var item in searcher.Get())
        {
            cpu.Name = item["Name"]?.ToString() ?? "Desconhecido";
            cpu.CoreCount = Convert.ToInt32(item["NumberOfCores"] ?? 0);
            cpu.ThreadCount = Convert.ToInt32(item["NumberOfLogicalProcessors"] ?? 0);
            cpu.BaseClockGhz = Convert.ToSingle(item["MaxClockSpeed"] ?? 0) / 1000f;
        }

        return cpu;
    }

    private static CpuInfo GetCpuDynamicInfo(CpuInfo cpu)
    {
        var computer = new Computer
        {
            IsCpuEnabled = true
        };
        computer.Open();

        foreach (var hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Cpu)
            {
                hardware.Update();

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Package"))
                        cpu.TemperatureCelsius = sensor.Value ?? 0;
                    if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                        cpu.UsagePercentage = sensor.Value ?? 0;
                    if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
                        cpu.CurrentClockGhz = (sensor.Value ?? 0) / 1000f;
                }
            }
        }
        computer.Close();
        return cpu;
    }

    public static CpuInfo GetCpuInfo()
    {
        var cpu = GetCpuStaticInfo();
        cpu = GetCpuDynamicInfo(cpu);
        return cpu;
    }
}
