using GarageCoreMVC.Common;
using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    [Serializable]
    public class Spot
    {
        [Key]
        [Required(ErrorMessage = DefaultValues.SpotIDEmptyMessage)]
        [RegularExpression(Constants.SpotIDRegex, ErrorMessage=DefaultValues.SpotIDValidatorMessage)]
        public string ID { get; set; }
        public int Capacity { get; set; }
        public bool Occupied { get; set; } = false;

        public Spot() { }
        public Spot(string? ID, int Capacity)
        {
            this.ID = ID;
            this.Capacity = Capacity;
        }

        public override string ToString()
        {
            return $"{ID} [ Capacity: {Capacity} wheeler ]";
        }
    }
}
