namespace CPUXRay.Models;

public class MotherboardInfo
{
    public MotherboardInfo()
    {
    }
    public MotherboardInfo(string fabricantePlacaMae, string modeloPlacaMae)
    {
        FabricantePlacaMae = fabricantePlacaMae;
        ModeloPlacaMae = modeloPlacaMae;
    }

    public string FabricantePlacaMae { get; set; } = string.Empty;
    public string ModeloPlacaMae { get; set; } = string.Empty;
}
