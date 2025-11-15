namespace HardwareMonitor.Models
{
    internal class PlacaMae
    {
        public PlacaMae()
        {
        }   
        public PlacaMae(string fabricantePlacaMae, string modeloPlacaMae)
        {
            this.fabricantePlacaMae = fabricantePlacaMae;
            this.modeloPlacaMae = modeloPlacaMae;
        }

        public string fabricantePlacaMae { get; set; }
        public string modeloPlacaMae { get; set; }
    }
}
