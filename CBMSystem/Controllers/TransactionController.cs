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
            var transactions = _transactionRepository.GetAll();
            return View(transactions);
        }

        public IActionResult RecentTrans()
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
                transaction.CreatedBy = User.Identity?.Name ?? "System"; 
                _transactionRepository.Add(transaction);
                TempData["SuccessMessage"] = "Transaction Successed!";
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
        
        [HttpGet]
        public IActionResult Details(int id)
        {
            var transaction = _transactionRepository.GetById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var transaction = _transactionRepository.GetById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _transactionRepository.Delete(id);
            TempData["SuccessMessage"] = "Transaction Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}

