using GarageCoreMVC.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageCoreMVC.Models
{
    public class ParkingTransaction
    {
        [Key]
        public string? ID { get; set; }
        public DateTime ParkStart { get; set; }
        public DateTime ParkEnd { get; set; } = DateTime.MaxValue;
        public ServiceT Service { get; set; }

        [ForeignKey("Vehicle")]
        public string? VehicleID { get; set; }
        public virtual Vehicle? vehicle { get; set; }

        [ForeignKey("Spot")]
        public string? SpotID { get; set; }
        public virtual Spot? spot { get; set; }
    }
}
