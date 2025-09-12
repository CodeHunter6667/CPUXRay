using System.Collections.Generic;
using System.IO;
using System.Management;

namespace CPUXRay.Models;

public class StorageInfo
{
    public string DriverLetter { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ulong TotalSpaceGB { get; set; }
    public ulong FreeSpaceGB { get; set; }
    public float ReadSpeedMBps { get; set; }
    public float WriteSpeedMBps { get; set; }

    private static List<StorageInfo> GetStorageInfo()
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
