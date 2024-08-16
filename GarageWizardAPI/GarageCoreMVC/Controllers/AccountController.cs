using Microsoft.AspNetCore.Mvc;
using GarageCoreMVC.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GarageCoreMVC.Common;
using GarageCoreMVC.Common.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using GarageCoreMVC.Common.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using GarageCoreAPI.Models;
using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userM;
        //private readonly SignInManager<User> _signM;

        public AccountController(IMapper mapper, UserManager<User> userM) // To use sign-in manager, inject here !
        {
            _mapper = mapper;
            _userM = userM;
            //_signM = signM;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(Urls.RegisterAccount)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody] UserRegistration rUser)
        {
            if (rUser == null)
            {
                return BadRequest(DefaultValues.UserRegisterDtoUnavailableMessage);
            }
            var user = _mapper.Map<User>(rUser);
            var result = await _userM.CreateAsync(user, rUser.Password);
            if (!result.Succeeded)
            {
                return Json(new AuthResponseMessage{ Registered = false, Message = DefaultValues.DuplicateUserFailMessage });
            }
            // Any user that register is a vistor as default
            await _userM.AddToRoleAsync(user, "Visitor");
            return Json(new AuthResponseMessage{ Registered = true, Message = $"User: {user.Name} is successfully registered !!" });
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [Route(Urls.LoginToAccount)]
        public async Task<IActionResult> Login([FromBody] UserLogin lUser)
        {
            if (lUser == null)
            {
                return BadRequest(DefaultValues.UserLoginDtoUnavailableMessage);
            }
            var user = await _userM.FindByEmailAsync(lUser.Email);
            if (user != null && await _userM.CheckPasswordAsync(user, lUser.Password))
            {
                var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                //var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));
                IList<string> roles = await _userM.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
                var token = JWTHandler.GenerateSecurityToken(identity, GlobalConfig.JWTExpiryHours);

                //// Identity sign in -------------------------------------------------------------
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                //    new ClaimsPrincipal(identity),
                //    // Add timeout
                //    new AuthenticationProperties
                //    {
                //        // If remember me is enabled, the login session persists even after the browser is closed !
                //        IsPersistent = lUser.RememberMe,
                //        ExpiresUtc = DateTime.UtcNow.AddMinutes(GlobalConfig.JWTExpiryHours),
                //        //RedirectUri = "/"
                //    }
                //);
                //// ------------------------------------------------------------------------------

                // Send encrypted token in response to the credentials from the user request
                Response.Cookies.Append(GlobalConfig.GeneratedTokenCookieName, token);
                return Json(new AuthResponseMessage{ LoggedIn = true, Token = token, Username = user.UserName, Message = DefaultValues.LoginSuccessMessage });
            }
            else
            {
                return Json(new AuthResponseMessage{ LoggedIn = false, Message = DefaultValues.LoginFailedMessage });
            }
        }

        [HttpPost]
        [Authorize]
        [ExcludeFromCodeCoverage]
        [Route(Urls.LogoutOfAccount)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //await _signM.SignOutAsync();
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            Response.Cookies.Delete(GlobalConfig.GeneratedTokenCookieName);
            return Json(new AuthResponseMessage{ LoggedIn = false, Message = DefaultValues.LoggedOutMessage });
        }

        [HttpGet]
        [Authorize(Roles = "Sudo")]
        [Route(Urls.GetAllUsers)]
        public async Task<ActionResult> GetAllUsers()
        {
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in _userM.Users.ToList())
            {
                var roles = await _userM.GetRolesAsync(user);
                userDtos.Add(
                    new UserDto
                    {
                        Name = user.Name,
                        Email = user.Email,
                        RememberMe = user.RememberMe,
                        Role = roles[0]
                    }
                );
            }
            return Json(userDtos);
        }

        [HttpGet]
        [Route(Urls.GetCurrentUser)]
        public async Task<ActionResult> GetCurrentUser()
        {
            UserDto? userDto = null;
            if (HttpContext.User.Identity != null)
            {
                var currentUser = await _userM.FindByEmailAsync(HttpContext.User.Identity.Name ?? "");
                if (currentUser != null)
                {
                    var roles = await _userM.GetRolesAsync(currentUser);
                    userDto = new UserDto
                    {
                        Name = currentUser.Name,
                        Email = currentUser.Email,
                        RememberMe = currentUser.RememberMe,
                        Role = roles[0]
                    };
                    return Json(userDto);
                }
            }
            return Json(new ResponseMessage { Success = false, Message = DefaultValues.NoUserAuthenticatedYetMessage, StatusCode = (int)HttpStatusCode.BadRequest });
        }

        [HttpPost]
        [Authorize]
        [Route(Urls.ChangeUserPassword)]
        public async Task<ActionResult> ChangeUserPassword([FromBody] UserChangeDto changeDto)
        {
            if (changeDto == null) return BadRequest(DefaultValues.PasswordChangeDtoUnavailableMessage);
            else
            {
                var currentUser = await _userM.FindByEmailAsync(HttpContext.User?.Identity?.Name ?? "");
                if (currentUser == null) return BadRequest(DefaultValues.PasswordChangeRestrictedMessage);
                else
                {
                    if (await _userM.CheckPasswordAsync(currentUser, changeDto.OldPassword))
                    {
                        await _userM.ChangePasswordAsync(currentUser, changeDto.OldPassword, changeDto.NewPassword);
                        return Json(new ResponseMessage{ Success = true, Message = $"Password for user: {currentUser.Name} is successfully changed !" });
                    }
                    return Json(new ResponseMessage{ Success = false, Message = DefaultValues.UserPasswordWrongMessage});
                }
            }
        }

        //[HttpGet]
        //[Authorize]
        //[Route(Urls.DenyAccess)]
        //public IActionResult AccessDenied()
        //{
        //    return Json(new { notAuthorized = true, message = DefaultValues.AccessDenialMessage });
        //}

        //[HttpGet]
        //[Route(Urls.ValidateClientApp)]
        //public ActionResult ValidateClientToken(string token)
        //{
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        var unique_name = JWTHandler.ValidateSecurityToken(token, JwtRegisteredClaimNames.UniqueName);
        //        return Json(new
        //        {
        //            name = unique_name,
        //            authenticated = true
        //        });
        //    }
        //    return Json(new
        //    {
        //        name = "null",
        //        authenticated = false
        //    });
        //}
    }
}