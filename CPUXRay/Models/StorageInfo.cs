namespace CPUXRay.Models;

public class StorageInfo
{
    public string Model { get; set; } = string.Empty;
    public string InterfaceType { get; set; } = string.Empty;
    public long TotalSize { get; set; }
    public long FreeSpace { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string FirmwareVersion { get; set; } = string.Empty;
    public int Temperature { get; set; }
    public bool IsSSD { get; set; }
}
