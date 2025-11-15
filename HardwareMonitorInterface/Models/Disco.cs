namespace HardwareMonitor.Models
{
    internal class Disco
    {
        public Disco() { }

        public Disco(string nomeDisco, double capacidadeTotalGBDisco, double espacoLivreGBDisco, int percentualUsoDisco)
        {
            this.nomeDisco = nomeDisco;
            this.capacidadeTotalGBDisco = capacidadeTotalGBDisco;
            this.espacoLivreGBDisco = espacoLivreGBDisco;
            this.percentualUsoDisco = percentualUsoDisco;
        }

        public string nomeDisco { get; set; }
        public double capacidadeTotalGBDisco { get; set; }
        public double espacoLivreGBDisco { get; set; }
        public int percentualUsoDisco { get; set; }
    }
}
