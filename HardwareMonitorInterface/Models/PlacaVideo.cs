namespace HardwareMonitor.Models
{
    internal class PlacaVideo
    {
        public PlacaVideo()
        {
        } 

        public PlacaVideo(string nomePlacaVideo, int memoriaTotalMBPlacaVideo, string versaoDriverPlacaVideo)
        {
            this.nomePlacaVideo = nomePlacaVideo;
            this.memoriaTotalMBPlacaVideo = memoriaTotalMBPlacaVideo;
            this.versaoDriverPlacaVideo = versaoDriverPlacaVideo;
        }

        public string nomePlacaVideo {  get; set; }
        public int memoriaTotalMBPlacaVideo {  get; set; }
        public string versaoDriverPlacaVideo {  get; set; }
    }
}
