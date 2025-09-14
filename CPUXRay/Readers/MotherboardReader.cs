using CPUXRay.Models;
using System;
using System.Management;

namespace CPUXRay.Readers;

public static class MotherboardReader
{
    public static MotherboardInfo GetMotherboardInfo()
    {
        var board = new MotherboardInfo();

        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
        foreach (var item in searcher.Get())
        {
            board.Manufacturer = item["Manufacturer"]?.ToString() ?? string.Empty;
            board.Model = item["Model"]?.ToString() ?? string.Empty;
            board.Product = item["Product"]?.ToString() ?? string.Empty;
            board.SerialNumber = item["SerialNumber"]?.ToString() ?? string.Empty;
        }
        return board;
    }

    public static void EnrichWithBiosInfo(MotherboardInfo board)
    {
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        foreach (var item in searcher.Get())
        {
            board.BiosVersion = item["SMBIOSBIOSVersion"]?.ToString() ?? string.Empty;
            if (DateTime.TryParse(item["ReleaseDate"]?.ToString(), out var biosDate))
                board.BiosReleaseDate = biosDate;
        }
    }

    public static void EnrichWithMemorySlots(MotherboardInfo board)
    {
        int slots = 0;
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
        foreach (var item in searcher.Get())
        {
            slots++;
        }
        board.MemorySlots = slots;
    }
}
