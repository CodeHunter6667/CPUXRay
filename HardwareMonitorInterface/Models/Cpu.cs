namespace HardwareMonitorInterface.Models;

public class Cpu
{
    public Cpu() { 
    }
    public Cpu(string nomeProcessador, int nucleosFisicosProcessador, int nucleosLogicosProcessador, int frequenciaMaximaMHzProcessador, int usoPorcentagemProcessador)
    {
        NomeProcessador = nomeProcessador;
        NucleosFisicosProcessador = nucleosFisicosProcessador;
        NucleosLogicosProcessador = nucleosLogicosProcessador;
        FrequenciaMaximaMHzProcessador = frequenciaMaximaMHzProcessador;
        UsoPorcentagemProcessador = usoPorcentagemProcessador;
        UsoPorcentagemPorNucleo = new System.Collections.Generic.List<int>();
    }

    public string NomeProcessador { get; set; } = string.Empty;
    public int NucleosFisicosProcessador { get; set; }
    public int NucleosLogicosProcessador { get; set; }
    public int FrequenciaMaximaMHzProcessador { get; set; }
    public int UsoPorcentagemProcessador { get; set; }

    // NOVO: uso por núcleo lógico (0..N-1)
    public System.Collections.Generic.List<int> UsoPorcentagemPorNucleo { get; set; } = new();
}
