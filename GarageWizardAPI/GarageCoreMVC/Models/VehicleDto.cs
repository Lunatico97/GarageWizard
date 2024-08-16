namespace GarageCoreMVC.Models
{
    public class VehicleDto
    {
        public string? RegID { get; set; }
        public string? Name { get; set; }
        public string? Vendor { get; set; }
        public int Wheels { get; set; }
        public string? ParkedSpotID { get; set; }
        public string? ParkedSince { get; set; }
        public string? ParkedFor { get; set; }
    }
}
