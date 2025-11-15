namespace HardwareMonitor.Models
{
    internal class Memoria
    {
        public Memoria()
        {
        }   
        public Memoria(int totalMBMemoria, int totalEmUsoMBMemoria, int totalMemoriaLivreMB, int usoPorcentagemMemoria)
        {
            this.totalMBMemoria = totalMBMemoria;
            this.totalEmUsoMBMemoria = totalEmUsoMBMemoria;
            this.totalLivreMBMemoria= totalMemoriaLivreMB;
            this.usoPorcentagemMemoria = usoPorcentagemMemoria;
        }

        public int totalMBMemoria { get; set; }
        public int totalEmUsoMBMemoria { get; set; }
        public int totalLivreMBMemoria{ get; set; }
        public int usoPorcentagemMemoria { get; set; }
    }
}
