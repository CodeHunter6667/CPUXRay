namespace HardwareMonitor.Models
{
    internal class Cpu
    {
        public Cpu() { 
        }
        public Cpu(string nomeProcessador, int nucleosFisicospProcessador, int nucleosLogicosProcessador, int frequenciaMaximaMHzProcessador, int usoPorcentagemProcessador)
        {
            this.nomeProcessador = nomeProcessador;
            this.nucleosFisicosProcessador = nucleosFisicosProcessador;
            this.nucleosLogicosProcessador = nucleosLogicosProcessador;
            this.frequenciaMaximaMHzProcessador = frequenciaMaximaMHzProcessador;
            this.usoPorcentagemProcessador = usoPorcentagemProcessador;
        }

        public string nomeProcessador { get; set; }
        public int nucleosFisicosProcessador { get; set; }
        public int nucleosLogicosProcessador { get; set; }
        public int frequenciaMaximaMHzProcessador { get; set; }
        public int usoPorcentagemProcessador { get; set; }
    }
}
