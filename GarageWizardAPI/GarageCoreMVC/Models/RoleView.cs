using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    public class RoleView
    {
        [Required]
        [Display(Name="Role")]
        public string Name {  get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
