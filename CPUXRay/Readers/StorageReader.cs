using CPUXRay.Models;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace CPUXRay.Readers;

public static class StorageReader
{

    public static List<StorageInfo> GetStorageInfo()
    {
        var drivers = new List<StorageInfo>();

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady && drive.DriveType == DriveType.Fixed)
            {
                var info = new StorageInfo
                {
                    DriverLetter = drive.Name,
                    TotalSpaceGB = (ulong)(drive.TotalSize / (1024 * 1024 * 1024)),
                    FreeSpaceGB = (ulong)(drive.TotalFreeSpace / (1024 * 1024 * 1024)),
                };
                drivers.Add(info);
            }
        }
        return drivers;
    }

    public static void EnrichStorageInfo(List<StorageInfo> drives)
    {
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
        var diskIndex = 0;

        foreach (var disk in searcher.Get())
        {
            if (diskIndex < drives.Count)
            {
                drives[diskIndex].Model = disk["Model"]?.ToString() ?? "Unknown";
                drives[diskIndex].Type = disk["MediaType"]?.ToString() ?? "Unknown";
            }
            diskIndex++;
        }
    }
}
