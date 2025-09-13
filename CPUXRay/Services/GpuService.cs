using CPUXRay.Models;
using CPUXRay.Readers;

namespace CPUXRay.Services;

public static class GpuService
{
    public static GpuInfo GetGpuInfo()
    {
        return GpuReader.GetGpuInfo();
    }
}
