using System;

namespace CPUXRay.Models;

public class MotherboardInfo
{
    public string Manufacturer { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string BiosManufacturer { get; set; } = string.Empty;
    public string BiosVersion { get; set; } = string.Empty;
    public DateTime BiosReleaseDate { get; set; }
    public string Chipset { get; set; } = string.Empty;
}
