using System.Collections.Generic;

namespace CPUXRay.Models;

public class RamInfo
{
    public RamInfo()
    {
    }
    public RamInfo(int totalMBMemoria, int totalEmUsoMBMemoria, int totalMemoriaLivreMB, int usoPorcentagemMemoria)
    {
        TotalMBMemoria = totalMBMemoria;
        TotalEmUsoMBMemoria = totalEmUsoMBMemoria;
        TotalLivreMBMemoria = totalMemoriaLivreMB;
        UsoPorcentagemMemoria = usoPorcentagemMemoria;
    }

    public int TotalMBMemoria { get; set; }
    public int TotalEmUsoMBMemoria { get; set; }
    public int TotalLivreMBMemoria { get; set; }
    public int UsoPorcentagemMemoria { get; set; }
}
