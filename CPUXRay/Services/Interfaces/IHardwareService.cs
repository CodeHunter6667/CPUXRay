using CPUXRay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPUXRay.Services.Interfaces;

public interface IHardwareService
{
    Task<CpuInfo> GetCpuInfoAsync();
    Task<RamInfo> GetMemoryInfoAsync();
    Task<List<StorageInfo>> GetStorageInfoAsync();
    Task<List<GpuInfo>> GetGpuInfoAsync();
    Task<MotherboardInfo> GetMotherboardInfoAsync();
}
