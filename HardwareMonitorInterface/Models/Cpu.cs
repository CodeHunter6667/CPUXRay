namespace HardwareMonitorInterface.Models;

public class Cpu
{
    public Cpu() { 
    }
    public Cpu(string nomeProcessador, int nucleosFisicospProcessador, int nucleosLogicosProcessador, int frequenciaMaximaMHzProcessador, int usoPorcentagemProcessador)
    {
        NomeProcessador = nomeProcessador;
        NucleosFisicosProcessador = NucleosFisicosProcessador;
        NucleosLogicosProcessador = nucleosLogicosProcessador;
        FrequenciaMaximaMHzProcessador = frequenciaMaximaMHzProcessador;
        UsoPorcentagemProcessador = usoPorcentagemProcessador;
    }

    public string NomeProcessador { get; set; } = string.Empty;
    public int NucleosFisicosProcessador { get; set; }
    public int NucleosLogicosProcessador { get; set; }
    public int FrequenciaMaximaMHzProcessador { get; set; }
    public int UsoPorcentagemProcessador { get; set; }
}
