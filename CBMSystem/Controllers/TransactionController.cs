using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CBMSystem.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransaction _transactionRepository;

        public TransactionController(ITransaction transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IActionResult Index()
        {
            var transactions = _transactionRepository.GetRecent(10);
            return View(transactions);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var accountNumbers = _transactionRepository.GetAccountNumbers() ?? new List<string>();
            ViewBag.Accounts = accountNumbers.Select(acc => new SelectListItem
            {
                Value = acc,
                Text = acc
            }).ToList();
            /*.Select(a => a.AccountNumber).ToList();*/


            ViewBag.TransactionTypes = new List<string>
            {
            "Challan Payment", "Insurance Payment", "Loan Installment", "Deposit", "Withdrawal"
            };
            return View();
        }

        [HttpPost]
        public IActionResult Add(Transaction transaction)
        {
            if (transaction != null)
            {
                _transactionRepository.Add(transaction);
                return RedirectToAction("Index");
            }

            var accountNumbers = _transactionRepository.GetAccountNumbers();
            ViewBag.Accounts = accountNumbers.Select(acc => new SelectListItem
            {
                Value = acc,
                Text = acc
            }).ToList();

            ViewBag.TransactionTypes = new List<string>
            {
            "Challan Payment", "Insurance Payment", "Loan Installment", "Deposit", "Withdrawal"
            };
            return View(transaction);
        }
    }
}

