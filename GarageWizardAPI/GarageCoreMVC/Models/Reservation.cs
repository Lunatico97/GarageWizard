using GarageCoreMVC.Common.Enums;

namespace GarageCoreMVC.Models
{
    public class Reservation
    {
        public string? spotID {  get; set; }
        public string? vehicleID { get; set; }
        public int service {  get; set; }
    }
}
