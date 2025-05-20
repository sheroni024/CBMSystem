using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using CBMSystem.Repository.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CBMSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISearch _navSearch;
        public HomeController(ILogger<HomeController> logger, ISearch _navSearch) : this(logger)
        {
            this._navSearch = _navSearch;
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            string se= HttpContext.Session.GetString("UserEmail");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // FOR SEARCH
        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            var url = _navSearch.GetRedirectUrlForSearch(searchTerm);
            if (string.IsNullOrEmpty(url))
            {
                // Optionally, redirect to a NotFound or fallback view
                TempData["SearchMessage"] = "No matching page found.";
                return RedirectToAction("Dashboard", "User");
            }

            return Redirect(url);
        }

        [HttpGet]
        public JsonResult GetSearchSuggestions(string term)
        {
            var isAuthenticated = HttpContext.Session.GetString("UserEmail") != null;
            var allTerms = new List<string> { "AddCustomer", "DeleteTransaction", "Loan", "CustomerIndex", "TransactionIndex" };

            var suggestions = allTerms
                .Where(t => t.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Json(new { isAuthenticated, suggestions });
        }

    }
}
