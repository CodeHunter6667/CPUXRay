using CPUXRay.Models;
using CPUXRay.Readers;

namespace CPUXRay.Services;

public static class CpuService
{
    public static CpuInfo GetCpuInfo()
    {
        return CpuReader.GetCpuInfo();
    }
}
