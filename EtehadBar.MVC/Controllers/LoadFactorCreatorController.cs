using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtehadBar.MVC.Controllers
{
    [Authorize(Roles = "Admin,Milad")]
    public class LoadFactorCreatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
