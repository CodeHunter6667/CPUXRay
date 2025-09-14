using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Linq;

namespace CPUXRay.Hardware;

public static class SensorManager
{
    private static readonly Computer _computer;

    static SensorManager()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsStorageEnabled = true,
            IsMotherboardEnabled = true
        };
        _computer.Open();
    }

    public static IEnumerable<IHardware> GetHardware(HardwareType type)
    {
        return _computer.Hardware.Where(h => h.HardwareType == type);
    }

    public static void UpdateAll()
    {
        foreach (var hardware in _computer.Hardware)
        {
            hardware.Update();
        }
    }
}
