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
}
