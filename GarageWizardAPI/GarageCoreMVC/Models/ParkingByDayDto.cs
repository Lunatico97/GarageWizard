namespace GarageCoreAPI.Models
{
    public class ParkingByDayDto
    {
        public string Day { get; set; } = string.Empty;
        public double AmountToll { get; set; }
        public double AmountRepairs { get; set; }
    }
}
