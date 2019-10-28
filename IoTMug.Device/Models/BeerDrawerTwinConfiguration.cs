namespace IoTMug.Device.Models
{
    public class BeerDrawerTwinConfiguration
    {
        public int RateOfFlow { get; set; }
        public string BeerName { get; set; }
        public int Quantity { get; set; }
        public BeerPrices Prices { get; set; }

    }

    public class BeerPrices
    {
        public decimal Half { get; set; }
        public decimal Pint { get; set; }
        public decimal Happy { get; set; }
    }
}
