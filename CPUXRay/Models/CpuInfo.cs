using LibreHardwareMonitor.Hardware;
using System;
using System.Management;

namespace CPUXRay.Models;

public class CpuInfo
{
    public string Name { get; set; } = string.Empty;
    public int CoreCount { get; set; }
    public int ThreadCount { get; set; }
    public float BaseClockGhz { get; set; }
    public float TemperatureCelsius { get; set; }
    public float CurrentClockGhz { get; set; }
    public float UsagePercentage { get; set; }
}
