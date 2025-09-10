using System;
using System.Management;
using System.Runtime.InteropServices;

namespace CPUXRay.Models;

public class RamInfo
{
    public ulong TotalMemoryMB { get; set; }
    public ulong UsedMemomryMB { get; set; }
    public ulong AvailableMemoryMB { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public int SpeedMHz { get; set; }
    public int SlotCount { get; set; }

    public static RamInfo GetRamInfo()
    {
        var ram = new RamInfo();
        ulong totalCapacity = 0;
        int slotCount = 0;
        int speed = 0;
        string type = string.Empty;

        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
        foreach (var item in searcher.Get())
        {
            totalCapacity += (ulong)item["Capacity"] / (1024 * 1024);
            slotCount++;
            speed = Convert.ToInt32(item["Speed"]);
            type = GetMemoryType(Convert.ToInt32(item["MemoryType"]));
        }

        ram.TotalMemoryMB = totalCapacity;
        ram.SlotCount = slotCount;
        ram.SpeedMHz = speed;
        ram.MemoryType = type;

        var (totalMB, availableMB) = MemoryReader.GetMemoryStatus();
        ram.AvailableMemoryMB = availableMB;
        ram.UsedMemomryMB = totalMB - availableMB;
        return ram;
    }

    public static string GetMemoryType(int typeCode)
    {
        return typeCode switch
        {
            20 => "DDR",
            21 => "DDR2",
            22 => "DDR2 FB-DIMM",
            24 => "DDR3",
            25 => "DDR4",
            26 => "DDR5",
            _ => "Unknown"
        };
    }

    public static class MemoryReader
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX buffer);

        public static (ulong totalMB, ulong availableMB) GetMemoryStatus()
        {
            var status = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(status))
            {
                ulong total = status.ullTotalPhys / (1024 * 1024);
                ulong available = status.ullAvailPhys / (1024 * 1024);
                return (total, available);
            }
            throw new InvalidOperationException("Falha ao obter status da memória.");
        }
    }

}
