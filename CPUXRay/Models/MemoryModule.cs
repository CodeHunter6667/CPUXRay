namespace CPUXRay.Models;

public class MemoryModule
{
    public string Manufacturer { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public long Capacity { get; set; } // Bytes
    public int Speed { get; set; } // MHz
    public string FormFactor { get; set; } = string.Empty;
    public string MemoryType { get; set; } = string.Empty;
    public string BankLabel { get; set; } = string.Empty;
}
