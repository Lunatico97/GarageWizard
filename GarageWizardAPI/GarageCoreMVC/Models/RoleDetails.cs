using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    public class RoleDetails
    {
        public required string ID { get; set; }
        [Required(ErrorMessage = "Role name can't be empty !")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IList<string> AccessUserIDs { get; set; } = new List<string>();
    }
}
