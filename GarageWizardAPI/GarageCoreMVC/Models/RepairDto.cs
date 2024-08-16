namespace GarageCoreMVC.Models
{
    public class RepairDto
    {
        public string? vehicleID { get; set; }
        public List<Job> jobs { get; set; } = [];
    }
}
