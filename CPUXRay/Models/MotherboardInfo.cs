using System;

namespace CPUXRay.Models;

public class MotherboardInfo
{
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Chipset { get; set; } = string.Empty;
    public string BiosVersion { get; set; } = string.Empty;
    public DateTime BiosReleaseDate { get; set; }
    public int MemorySlots { get; set; }
    public string FormFactor { get; set; } = string.Empty;
}
