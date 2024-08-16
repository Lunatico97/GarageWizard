using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageCoreMVC.Controllers
{
    [Authorize(Roles = "Sudo")]
    public class AdminController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public AdminController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route(Urls.ListRoles)]
        public async Task<IActionResult> ListRole()
        { 
            List<Role> roles = await _roleManager.Roles.ToListAsync();
            return Json(roles);
        }

        [HttpPost]
        [Route(Urls.CreateRole)]
        public async Task<IActionResult> CreateRole(RoleView role)
        {
            // If role doesn't exist, create new role to store it in database context
            if(role != null)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    Role newRole = new Role() { Name = role.Name, Description = role.Description };
                    var result = await _roleManager.CreateAsync(newRole);
                    if (!result.Succeeded)
                    {
                        return Json(new ResponseMessage() { Success = false, Message = DefaultValues.RoleCreationFailedMessage });
                    }
                    return Json(new ResponseMessage() { Success = true, Message = DefaultValues.RoleCreationSuccessMessage });
                }
                return Json(new ResponseMessage() { Success = false, Message = DefaultValues.RoleExistsMessage });
            }
            return BadRequest(DefaultValues.RoleDetailsUnavailableMessage);
        }

        [HttpGet]
        [Route(Urls.GetRoleDetails)]
        public async Task<IActionResult> GetRoleDetails([FromQuery] string roleID)
        {
            Role? role = await _roleManager.FindByIdAsync(roleID);
            if(role == null)
            {
                return NotFound($"The role with <{roleID}> doesn't exist !");
            }
            var details = new RoleDetails
            {
                ID = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            foreach(var user in _userManager.Users.ToList())
            {
                if(await _userManager.IsInRoleAsync(user, role.Name ?? ""))
                {
                    if(!string.IsNullOrWhiteSpace(user.UserName))
                        details.AccessUserIDs.Add(user.UserName);
                }
            }
            return Json(details);
;       }

        [HttpPost]
        [Route(Urls.UpdateRole)]
        public async Task<IActionResult> EditRole([FromBody] RoleDetails roleDetails)
        {
            if (roleDetails != null)
            {
                var role = await _roleManager.FindByIdAsync(roleDetails.ID);
                if (role == null)
                {
                    return NotFound($"Role with ID <{roleDetails.ID}> cannot be found! ");
                }
                else
                {
                    role.Name = roleDetails.Name;
                    role.Description = roleDetails.Description;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return Json(new ResponseMessage{ Success = true, Message = $"{role.Name} role is updated successfully !" });
                    }
                    return Json(new ResponseMessage{ Success = false, Message = $"{role.Name} cannot be updated properly !"}); 
                }
            }
            return BadRequest(DefaultValues.RoleDetailsUnavailableMessage);
        }

 

        [HttpPost]
        [Route(Urls.EditRoleAccess)]
        public async Task<IActionResult> EditRoleAccess([FromBody] List<RoleAccess> accessIDs)
        {
            if(accessIDs == null || accessIDs.Count() == 0)
            {
                return BadRequest(DefaultValues.RoleAccessListUnavailableMessage);
            }
            else
            {
                foreach(var ID in accessIDs)
                {
                    var user = await _userManager.FindByEmailAsync(ID.UserID); // In my case, unique identification is email !
                    if (user == null) return NotFound(DefaultValues.UserInAccessUnavailableMessage);
                    if(!ID.IsAuthorized && await _userManager.IsInRoleAsync(user, ID.Role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, ID.Role);
                    }
                    else if (ID.IsAuthorized && !await _userManager.IsInRoleAsync(user, ID.Role))
                    {
                        await _userManager.AddToRoleAsync(user, ID.Role);
                        // Remove other roles so that one can have a single role only !
                        var userRoles = await _userManager.GetRolesAsync(user);
                        foreach (var role in userRoles)
                        {
                            if(role != ID.Role)
                            {
                                await _userManager.RemoveFromRoleAsync(user, role);
                            }
                        }
                    }
                    else continue;
                }
                return Json(new ResponseMessage{ Success = true, Message = $"Role access list for {accessIDs[0].Role} is updated !" });
            }
        }
    }
}
