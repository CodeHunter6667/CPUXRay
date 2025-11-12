using System.Collections.Generic;

namespace CPUXRay.Models;

public class RamInfo
{
    public long TotalPhysicalMemory { get; set; } // Bytes
    public long AvailableMemory { get; set; }
    public double UsagePercentage { get; set; }
    public List<MemoryModule> Modules { get; set; } = [];
}
