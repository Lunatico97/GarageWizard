using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GarageCoreMVC.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public override required string? Email { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}
