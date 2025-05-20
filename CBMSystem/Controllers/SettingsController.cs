using Microsoft.AspNetCore.Mvc;

namespace CBMSystem.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
