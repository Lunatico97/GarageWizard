using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

    namespace GarageCoreMVC.Models
    {
        public class UserRegistration 
        {
            public string Name { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            public required string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
        }
    }
