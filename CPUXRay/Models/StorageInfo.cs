namespace CPUXRay.Models;

public class StorageInfo
{
    public StorageInfo() { }

    public StorageInfo(string nomeDisco, double capacidadeTotalGBDisco, double espacoLivreGBDisco, int percentualUsoDisco, string modelo)
    {
        NomeDisco = nomeDisco;
        CapacidadeTotalGBDisco = capacidadeTotalGBDisco;
        EspacoLivreGBDisco = espacoLivreGBDisco;
        PercentualUsoDisco = percentualUsoDisco;
        Modelo = modelo;
    }

    public string NomeDisco { get; set; } = string.Empty;
    public double CapacidadeTotalGBDisco { get; set; }
    public double EspacoLivreGBDisco { get; set; }
    public int PercentualUsoDisco { get; set; }
    public string Modelo { get; set; } = string.Empty;
}
