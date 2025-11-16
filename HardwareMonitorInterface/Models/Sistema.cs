using System.Collections.Generic;

namespace HardwareMonitorInterface.Models;

public class Sistema
{
    public Cpu Cpu { get; set; } = new();
    public Memoria Memoria { get; set; } = new();
    public List<Disco> Discos { get; set; } = new();
    public PlacaMae PlacaMae { get; set; } = new();
    public PlacaVideo PlacaVideo { get; set; } = new();
    public Sistema() { 
    }
    public Sistema(Cpu cpu, Memoria memoria, List<Disco> discos, PlacaMae placaMae, PlacaVideo placaVideo)
    {
        Cpu = cpu;
        Memoria = memoria;
        Discos = discos;
        PlacaMae = placaMae;
        PlacaVideo = placaVideo;
    }
}
