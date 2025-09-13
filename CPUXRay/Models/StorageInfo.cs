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
}
