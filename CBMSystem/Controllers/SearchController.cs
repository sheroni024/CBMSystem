using CBMSystem.Models;
using CBMSystem.Repository.Service;
using Microsoft.AspNetCore.Mvc;

namespace CBMSystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchRepository searchRepository;
        public SearchController (SearchRepository searchRepository)
        {
            this.searchRepository = searchRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
