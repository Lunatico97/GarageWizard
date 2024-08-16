using GarageCoreMVC.Common;
using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    [Serializable]
    public class Vehicle
    {
        [Required(ErrorMessage = DefaultValues.VehicleStringEmptyMessage)]
        [RegularExpression(Constants.VehicleStringRegex, ErrorMessage = DefaultValues.VehicleStringRegexMessage)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = DefaultValues.VehicleStringEmptyMessage)]
        [RegularExpression(Constants.VehicleStringRegex, ErrorMessage = DefaultValues.VehicleStringRegexMessage)]
        public string Vendor { get; set; } = string.Empty;

        public int Wheels { get; set; }

        [Key]
        [Required]
        [RegularExpression(Constants.VehicleIDRegex, ErrorMessage= DefaultValues.VehicleIDValidatorMessage)]
        public string? RegID { get; set; }

        public bool IsOnRepair { get; set; } = false;

        public override string ToString()
        {
            return $"{RegID} | {Vendor} - {Name} ({Wheels}) ";
        }
    }
}

