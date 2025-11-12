using CPUXRay.Hardware;
using CPUXRay.Models;
using CPUXRay.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace CPUXRay.Services;

public class HardwareMonitorService : IHardwareService
{
    public async Task<CpuInfo> GetCpuInfoAsync()
    {
        return await Task.Run(() =>
        {
            var cpuInfo = new CpuInfo();

            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpuInfo.Name = obj["Name"]?.ToString();
                cpuInfo.Manufacturer = obj["Manufacturer"]?.ToString();
                cpuInfo.Architecture = GetArchitecture(
                    Convert.ToInt32(obj["Architecture"]));
                cpuInfo.Cores = Convert.ToInt32(obj["NumberOfCores"]);
                cpuInfo.LogicalProcessors = Convert.ToInt32(
                    obj["NumberOfLogicalProcessors"]);
                cpuInfo.MaxClockSpeed = Convert.ToDouble(
                    obj["MaxClockSpeed"]);
                cpuInfo.CurrentClockSpeed = Convert.ToDouble(
                    obj["CurrentClockSpeed"]);
                cpuInfo.Socket = obj["SocketDesignation"]?.ToString();
                cpuInfo.ProcessorId = obj["ProcessorId"]?.ToString();
                cpuInfo.L2CacheSize = Convert.ToInt32(
                    obj["L2CacheSize"]);
                cpuInfo.L3CacheSize = Convert.ToInt32(
                    obj["L3CacheSize"]);
            }

            // Performance Counter para uso
            using var cpuCounter = new PerformanceCounter(
                "Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(100);
            cpuInfo.Usage = cpuCounter.NextValue();

            return cpuInfo;
        });
    }

    public async Task<List<GpuInfo>> GetGpuInfoAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<RamInfo> GetMemoryInfoAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<MotherboardInfo> GetMotherboardInfoAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<List<StorageInfo>> GetStorageInfoAsync()
    {
        throw new System.NotImplementedException();
    }

    private string GetArchitecture(int arch)
    {
        return arch switch
        {
            0 => "x86",
            1 => "MIPS",
            2 => "Alpha",
            3 => "PowerPC",
            5 => "ARM",
            6 => "Itanium",
            9 => "x64",
            12 => "ARM64",
            _ => "Unknown"
        };
    }
}
