namespace HardwareMonitorInterface.Models
{
    public class Disco
    {
        public Disco() { }

        public Disco(string nomeDisco, double capacidadeTotalGBDisco, double espacoLivreGBDisco, int percentualUsoDisco)
        {
            NomeDisco = nomeDisco;
            CapacidadeTotalGBDisco = capacidadeTotalGBDisco;
            EspacoLivreGBDisco = espacoLivreGBDisco;
            PercentualUsoDisco = percentualUsoDisco;
        }

        public string NomeDisco { get; set; } = string.Empty;
        public double CapacidadeTotalGBDisco { get; set; }
        public double EspacoLivreGBDisco { get; set; }
        public int PercentualUsoDisco { get; set; }
    }
}
