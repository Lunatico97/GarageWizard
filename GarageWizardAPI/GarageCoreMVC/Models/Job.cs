using GarageCoreMVC.Common;
using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    public class Job
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = DefaultValues.JobStringEmptyMessage)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9- ]*$", ErrorMessage = DefaultValues.JobStringRegexMessage)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;

        [Required(ErrorMessage = DefaultValues.JobChargeableMessage)]
        public int Charge {  get; set; }
        public double Hours { get; set; }
        public string JobImagePath { get; set; } = "";
    }
}
