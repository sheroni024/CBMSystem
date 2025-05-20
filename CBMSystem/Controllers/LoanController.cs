using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using CBMSystem.Repository.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CBMSystem.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoan _loanRepository;

        public LoanController(ILoan loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public IActionResult Index()
        {
            var loans = _loanRepository.GetAll();
            return View(loans);
        }

        public IActionResult Details(long id)
        {
            var loan = _loanRepository.GetById(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        public IActionResult Create()
        {
            var accountNumbers = _loanRepository.GetAccountNumbers();
            ViewBag.Accounts = accountNumbers.Select(acc => new SelectListItem
            {
                Value = acc,
                Text = acc
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Loan loan)
        {
            //if (ModelState.IsValid)
            //{
            //    var accountNumbers = _loanRepository.GetAccountNumbers() ?? new List<string>();
            //    ViewBag.Accounts = accountNumbers.Select(acc => new SelectListItem
            //    {
            //        Value = acc,
            //        Text = acc
            //    }).ToList();

            //    _loanRepository.Add(loan);
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(loan);

            if (loan != null)
            {
                loan.CreatedBy = User.Identity?.Name ?? "System";
                _loanRepository.Add(loan);
                TempData["SuccessMessage"] = "Loan Registered Successfully!";
                return RedirectToAction("Index");
            }

            // Always populate ViewBag.Accounts for the view to work properly
            var accountNumbers = _loanRepository.GetAccountNumbers();
            ViewBag.Accounts = accountNumbers.Select(acc => new SelectListItem
            {
                Value = acc,
                Text = acc
            }).ToList();

            return View(loan);
        }

        public IActionResult Edit(long id)
        {
            var loan = _loanRepository.GetById(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Loan loan)
        {
            //if (id != loan.LoanId)
            //{
            //    return NotFound();
            //}

            if (loan != null)
            {
                _loanRepository.Update(loan);
                TempData["SuccessMessage"] = "Loan Updated Successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(loan);
        }

        public IActionResult Delete(long id)
        {
            var loan = _loanRepository.GetById(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            _loanRepository.Delete(id);
            TempData["SuccessMessage"] = "Loan Deleted Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
