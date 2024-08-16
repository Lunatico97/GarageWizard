using System.Security.Permissions;

namespace GarageCoreMVC.Models
{
    public class RoleAccess
    {
        public string UserID { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
    }
}
