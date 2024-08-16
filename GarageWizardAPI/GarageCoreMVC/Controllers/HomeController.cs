using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using Microsoft.AspNetCore.Mvc;

namespace GarageCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        [Route(Urls.Home)]
        public ActionResult Index()
        {
            return Json(new ResponseMessage() { Success = true, Message = DefaultValues.WelcomeMessage, StatusCode = 200 });
        }
    }
}
