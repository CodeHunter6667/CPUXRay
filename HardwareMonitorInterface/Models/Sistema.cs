namespace HardwareMonitor.Models
{
    internal class Sistema
    {
        public Cpu cpu { get; set; }
        public Memoria memoria { get; set; }
        public Disco disco { get; set; }
        public PlacaMae placaMae { get; set; }
        public PlacaVideo placaVideo { get; set; }
        public Sistema() { 
        }
        public Sistema(Cpu cpu, Memoria memoria, Disco disco, PlacaMae placaMae, PlacaVideo placaVideo)
        {
            this.cpu = cpu;
            this.memoria = memoria;
            this.disco = disco;
            this.placaMae = placaMae;
            this.placaVideo = placaVideo;
        }
    }
}
