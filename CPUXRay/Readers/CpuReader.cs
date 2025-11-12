using CPUXRay.Hardware;
using CPUXRay.Models;
using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;
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
            cpu.Name = item["Name"]?.ToString()?.Trim() ?? "Desconhecido";
            cpu.Manufacturer = item["Manufacturer"]?.ToString() ?? "Desconhecido";
            cpu.Architecture = GetArchitecture(Convert.ToUInt16(item["Architecture"] ?? 0));
            cpu.ProcessorId = item["ProcessorId"]?.ToString() ?? string.Empty;
            cpu.CoreCount = Convert.ToInt32(item["NumberOfCores"] ?? 0);
            cpu.ThreadCount = Convert.ToInt32(item["NumberOfLogicalProcessors"] ?? 0);
            cpu.MaxClockGhz = Convert.ToSingle(item["MaxClockSpeed"] ?? 0) / 1000f;
            cpu.L2CacheKB = Convert.ToInt32(item["L2CacheSize"] ?? 0);
            cpu.L3CacheKB = Convert.ToInt32(item["L3CacheSize"] ?? 0);
            cpu.Socket = item["SocketDesignation"]?.ToString() ?? "Desconhecido";
            cpu.VirtualizationEnabled = Convert.ToInt32(item["VirtualizationFirmwareEnabled"] ?? 0);
            
            // Clock atual via WMI como fallback
            cpu.CurrentClockGhz = Convert.ToSingle(item["CurrentClockSpeed"] ?? 0) / 1000f;
        }

        return cpu;
    }

    private static string GetArchitecture(ushort arch)
    {
        return arch switch
        {
            0 => "x86",
            1 => "MIPS",
            2 => "Alpha",
            3 => "PowerPC",
            5 => "ARM",
            6 => "ia64",
            9 => "x64",
            12 => "ARM64",
            _ => "Desconhecida"
        };
    }

    private static CpuInfo GetCpuDynamicInfo(CpuInfo cpu)
    {
        var hardwareList = SensorManager.GetHardware(HardwareType.Cpu).ToList();
        
        if (!hardwareList.Any())
            return cpu;

        foreach (var hardware in hardwareList)
        {
            hardware.Update();

            // Busca clock médio de todos os cores
            var clockSensors = hardware.Sensors
                .Where(s => s.SensorType == SensorType.Clock && s.Name.Contains("Core"))
                .ToList();

            if (clockSensors.Any())
            {
                var avgClock = clockSensors.Average(s => s.Value ?? 0);
                if (avgClock > 0)
                    cpu.CurrentClockGhz = avgClock / 1000f;
            }

            // Busca temperatura
            var tempSensor = hardware.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && 
                    (s.Name.Contains("Package") || s.Name.Contains("Core Average") || s.Name.Contains("CPU")));
            
            if (tempSensor != null)
                cpu.TemperatureCelsius = tempSensor.Value ?? 0;

            // Busca uso da CPU
            var loadSensor = hardware.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Load && 
                    (s.Name.Contains("CPU Total") || s.Name == "Total"));
            
            if (loadSensor != null)
                cpu.UsagePercentage = loadSensor.Value ?? 0;

            // Calcula BaseClockGhz baseado na frequência atual se não foi definido
            if (cpu.BaseClockGhz == 0 && cpu.CurrentClockGhz > 0)
                cpu.BaseClockGhz = cpu.CurrentClockGhz;
        }

        return cpu;
    }

    public static CpuInfo GetCpuInfo()
    {
        var cpu = GetCpuStaticInfo();
        cpu = GetCpuDynamicInfo(cpu);
        return cpu;
    }
}
