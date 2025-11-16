namespace HardwareMonitorInterface.Models;

public class PlacaMae
{
    public PlacaMae()
    {
    }   
    public PlacaMae(string fabricantePlacaMae, string modeloPlacaMae)
    {
        FabricantePlacaMae = fabricantePlacaMae;
        ModeloPlacaMae = modeloPlacaMae;
    }

    public string FabricantePlacaMae { get; set; } = string.Empty;
    public string ModeloPlacaMae { get; set; } = string.Empty;
}
