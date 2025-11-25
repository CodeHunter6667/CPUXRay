using System.Collections.Generic;

namespace CPUXRay.Models;

public class SystemInfo
{
    public CpuInfo Cpu { get; set; } = new();
    public RamInfo Memoria { get; set; } = new();
    public List<StorageInfo> Discos { get; set; } = new();
    public MotherboardInfo PlacaMae { get; set; } = new();
    public List<GpuInfo> PlacasVideo { get; set; } = new();
    public SystemInfo()
    {
    }
    public SystemInfo(CpuInfo cpu, RamInfo memoria, List<StorageInfo> discos, MotherboardInfo placaMae, List<GpuInfo> placaVideo)
    {
        Cpu = cpu;
        Memoria = memoria;
        Discos = discos;
        PlacaMae = placaMae;
        PlacasVideo = placaVideo;
    }
}
