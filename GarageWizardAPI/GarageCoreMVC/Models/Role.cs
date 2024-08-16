using Microsoft.AspNetCore.Identity;

namespace GarageCoreMVC.Models
{
    public class Role : IdentityRole
    {
        // Name property already exists at default
        public string? Description { get; set; }
    }
}
