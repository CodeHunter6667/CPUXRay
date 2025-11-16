namespace HardwareMonitorInterface.Models;

public class PlacaVideo
{
    public PlacaVideo()
    {
    } 

    public PlacaVideo(string nomePlacaVideo, int memoriaTotalMBPlacaVideo, string versaoDriverPlacaVideo)
    {
        NomePlacaVideo = nomePlacaVideo;
        MemoriaTotalMBPlacaVideo = memoriaTotalMBPlacaVideo;
        VersaoDriverPlacaVideo = versaoDriverPlacaVideo;
    }

    public string NomePlacaVideo { get; set; } = string.Empty;
    public int MemoriaTotalMBPlacaVideo {  get; set; }
    public string VersaoDriverPlacaVideo {  get; set; } = string.Empty;
}
