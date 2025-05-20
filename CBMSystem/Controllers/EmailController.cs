using CBMSystem.Repository.Service;
using Microsoft.AspNetCore.Mvc;

namespace CBMSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;
        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(string toEmail, string subject, string message)
        {
            _emailService.SendEmail(toEmail, subject, message);
            ViewBag.Message = "Email sent successfully!";
            return View("Index");
        }
    }
}
