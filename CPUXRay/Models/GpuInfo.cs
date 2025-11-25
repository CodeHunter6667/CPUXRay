namespace CPUXRay.Models;

public class GpuInfo
{
    public GpuInfo()
    {
    }

    public GpuInfo(string nomePlacaVideo, long memoriaTotalMBPlacaVideo, string versaoDriverPlacaVideo, long memoriaEmUsoMBPlacaVideo = 0, long memoriaDisponivelMBPlacaVideo = 0)
    {
        NomePlacaVideo = nomePlacaVideo;
        MemoriaTotalMBPlacaVideo = memoriaTotalMBPlacaVideo;
        VersaoDriverPlacaVideo = versaoDriverPlacaVideo;
        MemoriaEmUsoMBPlacaVideo = memoriaEmUsoMBPlacaVideo;
        MemoriaDisponivelMBPlacaVideo = memoriaDisponivelMBPlacaVideo;
    }

    public string NomePlacaVideo { get; set; } = string.Empty;
    public long MemoriaTotalMBPlacaVideo { get; set; }
    public string VersaoDriverPlacaVideo { get; set; } = string.Empty;

    // Novo: uso e disponível em MB (tempo real quando DXGI permite)
    public long MemoriaEmUsoMBPlacaVideo { get; set; }
    public long MemoriaDisponivelMBPlacaVideo { get; set; }
}
