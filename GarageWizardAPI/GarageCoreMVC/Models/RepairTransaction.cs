using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    public class RepairTransaction
        {
            [Key]
            public string? ID { get; set; }
            public DateTime TransactionStamp { get; set; }

            [ForeignKey("Vehicle")]
            public string? VehicleID { get; set; }
            public virtual Vehicle? vehicle { get; set; }

            [ForeignKey("Job")]
            public string? JobID { get; set; }
            public virtual Job? job { get; set; }
            public bool IsCompleted { get; set; } = false;
            public int Charge { get; set; } = 1;
    }
}
