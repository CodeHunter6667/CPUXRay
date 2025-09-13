using CPUXRay.Models;
using CPUXRay.Readers;

namespace CPUXRay.Services;

public static class RamService
{
    public static RamInfo GetRamInfo()
    {
        return RamReader.GetRamInfo();
    }
}
