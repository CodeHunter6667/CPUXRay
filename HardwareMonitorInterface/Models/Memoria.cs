namespace HardwareMonitorInterface.Models;

public class Memoria
{
    public Memoria()
    {
    }   
    public Memoria(int totalMBMemoria, int totalEmUsoMBMemoria, int totalMemoriaLivreMB, int usoPorcentagemMemoria)
    {
        TotalMBMemoria = totalMBMemoria;
        TotalEmUsoMBMemoria = totalEmUsoMBMemoria;
        TotalLivreMBMemoria= totalMemoriaLivreMB;
        UsoPorcentagemMemoria = usoPorcentagemMemoria;
    }

    public int TotalMBMemoria { get; set; }
    public int TotalEmUsoMBMemoria { get; set; }
    public int TotalLivreMBMemoria{ get; set; }
    public int UsoPorcentagemMemoria { get; set; }
}
