using CBMSystem.Models;
using CBMSystem.Repository;
using CBMSystem.Repository.Interface;
using CBMSystem.Repository.Service;
using CBMSystem.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32.SafeHandles;
using NuGet.Protocol.Core.Types;
using System.Linq;
using System.Text;

namespace CBMSystem.Controllers
{
    public class CustomerAccountsController : Controller
    {
        private readonly ICustomers customersRepository;
        private readonly IWebHostEnvironment _environment;
        private SafeFileHandle profilePath;

        public CustomerAccountsController(ICustomers customersRepository, IWebHostEnvironment environment)
        {
            this.customersRepository = customersRepository;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult About()
        {
            ViewBag.Latitude = 20.0;
            ViewBag.Longitude = 80.0;
            return View();
        }

        // Display all customers
        [HttpGet("all")]
        public IActionResult GetAllCustomers()
        {
            var customerList = customersRepository.GetAllCustomers();
            return View(customerList); 
        }

        // Fetch single customer details by ID
        [HttpGet("details/{id}")]
        public IActionResult GetCustomerById(long id)
        {
            var customer = customersRepository.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }
            return View(customer); 
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CustomerDashboardViewModel model, IFormFile ProfileImageFile, IFormFile SignatureImageFile)
        {
            if (model!=null)
            {
                // Generate 11-digit AccountNumber
                string generatedAccountNumber = new Random().Next(100000000, int.MaxValue).ToString().PadLeft(11, '0');

                // Assign to both tables
                model.NwCustomer.AccountNumber = generatedAccountNumber;
                model.NwAccount.AccountNumber = generatedAccountNumber;
                // Generate unique folder for customer uploads
                var customerFolder = Path.Combine(_environment.WebRootPath, "uploads", "customers", Guid.NewGuid().ToString());
                if (!Directory.Exists(customerFolder))
                {
                    Directory.CreateDirectory(customerFolder);
                }

                // Save Profile Image
                if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
                {
                    var profileImagePath = Path.Combine(customerFolder, model.ProfileImageFile.FileName);
                    using (var stream = new FileStream(profileImagePath, FileMode.Create))
                    {
                        model.ProfileImageFile.CopyTo(stream);
                    }
                    model.NwCustomer.ProfileImage = $"{Path.GetFileName(customerFolder)}/{model.ProfileImageFile.FileName}";
                }

                // Save Signature Image
                if (model.SignatureImageFile != null && model.SignatureImageFile.Length > 0)
                {
                    var signatureImagePath = Path.Combine(customerFolder, model.SignatureImageFile.FileName);
                    using (var stream = new FileStream(signatureImagePath, FileMode.Create))
                    {
                        model.SignatureImageFile.CopyTo(stream);
                    }
                    model.NwCustomer.SignatureImage = $"{Path.GetFileName(customerFolder)}/{model.SignatureImageFile.FileName}";
                }

                // Save customer data to the database
                customersRepository.AddCustomer(model, User.Identity?.Name ?? "System");

                TempData["SuccessMessage"] = "Customer registered successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet("edit/{id}")]
        public IActionResult EditCustomer(int id)
        {
            var customer = customersRepository.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost("edit/{id}")]
        public IActionResult EditCustomer(CustomerAccount model)
        {
            if (model != null)
            {
                customersRepository.UpdateCustomer(model);
                TempData["SuccessMessage"] = "Customer Updated Successfully!";
                return RedirectToAction("GetAllCustomers");
            }
            return View(model);
        }

        [HttpGet("delete/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            customersRepository.DeleteCustomer(id);
            TempData["SuccessMessage"] = "Customer Deleted Successfully!";
            return RedirectToAction("GetAllCustomers");
        }

        // Fix for CS1729: 'PdfWriter' does not contain a constructor that takes 1 arguments
        // The issue occurs because the `PdfWriter` class does not have a constructor that accepts a single `MemoryStream` argument.
        // Instead, you need to use the `PdfWriter.GetInstance` method to create an instance of `PdfWriter`.

        [HttpGet]
        //public IActionResult DownloadTranPdf(long id, int months)
        //{
        //    var customer = customersRepository.GetCustomerById(id);
        //    if (customer == null)
        //    {
        //        return NotFound("Customer not found");
        //    }
        //    var model = new CustomerDashboardViewModel
        //    {
        //        NwCustomer = customer
        //    };
        //    return new Rotativa.AspNetCore.ViewAsPdf("CustomerPdf", model)
        //    {
        //        FileName = $"Customer_{id}.pdf",
        //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
        //        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        //    };
        //}


        public IActionResult DownloadTransactionsPdf(long customerId, int months)
        {
            try
            {
                var account = customersRepository.GetAccountByCustomerId(customerId);
                if (account == null)
                {
                    return NotFound("Account not found for this customer.");
                }

                var accountNumber = account.AccountNumber;

                // Get transactions using stored procedure
                var transactions = customersRepository.GetTransactionsByStoredProcedure(customerId, months);
                if (transactions == null || !transactions.Any()) 
                {
                    return NotFound("No transactions found.");
                }

                // Create PDF
                var model = new TransactionPdfViewModel
                {
                    CustomerId = customerId,
                    AccountNumber = accountNumber,
                    Months = months,
                    Transactions = (List<Transaction>)transactions
                };

                return new Rotativa.AspNetCore.ViewAsPdf("TransactionPdf", model)
                {
                    FileName = $"Transactions_Customer_{customerId}_{months}Months.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        //public IActionResult DownloadTransactionsPdf(int customerId, int months)
        //{
        //    try
        //    {
        //        // Protect against repository-level exceptions
        //        var customer = customersRepository.GetCustomerById(customerId);
        //        if (customer == null || string.IsNullOrEmpty(customer.AccountNumber))
        //        {
        //            return NotFound("Customer or account not found.");
        //        }

        //        var allTransactions = customersRepository.GetTransactionsByAccountNumber(customer.AccountNumber);
        //        if (allTransactions == null || !allTransactions.Any())
        //        {
        //            return BadRequest("This ID does not have any transactions yet.");
        //        }

        //        var fromDate = DateTime.Now.AddMonths(-months);
        //        var filteredTransactions = allTransactions
        //            .Where(t => t.TransactionDate >= fromDate)
        //            .OrderByDescending(t => t.TransactionDate)
        //            .ToList();

        //        if (!filteredTransactions.Any())
        //        {
        //            return NotFound("No transactions found for the selected period.");
        //        }

        //        // PDF generation
        //        using var ms = new MemoryStream();
        //        var document = new iTextSharp.text.Document();
        //        iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

        //        document.Open();
        //        document.Add(new iTextSharp.text.Paragraph($"Transaction History (Last {months} Month(s))"));
        //        document.Add(new iTextSharp.text.Paragraph($"Customer ID: {customerId}"));
        //        document.Add(new iTextSharp.text.Paragraph($"Account Number: {customer.AccountNumber}\n\n"));

        //        var table = new iTextSharp.text.pdf.PdfPTable(6) { WidthPercentage = 100 };
        //        table.AddCell("Txn ID");
        //        table.AddCell("Type");
        //        table.AddCell("Amount");
        //        table.AddCell("Date");
        //        table.AddCell("Ref No");
        //        table.AddCell("Description");

        //        foreach (var tx in filteredTransactions)
        //        {
        //            table.AddCell(tx.TransactionId.ToString());
        //            table.AddCell(tx.TransactionType);
        //            table.AddCell(tx.Amount.ToString("F2"));
        //            table.AddCell(tx.TransactionDate.ToString("yyyy-MM-dd"));
        //            table.AddCell(tx.ReferenceNumber);
        //            table.AddCell(tx.Description ?? "");
        //        }

        //        document.Add(table);
        //        document.Close();

        //        var fileName = $"Transactions_Customer_{customerId}_{months}Months.pdf";
        //        return File(ms.ToArray(), "application/pdf", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}





        [HttpPost]
        public IActionResult CreateDemat(CustomerDashboardViewModel model)
        {
            var accNo = model?.NwAccount?.AccountNumber;

            if (model == null || string.IsNullOrEmpty(accNo))
            {
                ModelState.AddModelError("NwAccount.AccountNumber", "Account Number is required.");
                return RedirectToAction("Index");
            }

            var customer = customersRepository.GetCustomerByAccountNumber(accNo);

            if (customer == null)
            {
                ModelState.AddModelError("", "No customer found for the provided Account Number.");
                return RedirectToAction("Index");
            }

            // Autofill CustomerId in the demat model
            model.NwDematAccount.CustomerId = customer.CustomerId;
            model.NwDematAccount.AccountNumber = accNo;

            customersRepository.CreateDematAccount(model.NwDematAccount);

            TempData["SuccessMessage"] = "Demat account created successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LinkAadhaarForm(int DematId, string AadhaarNumber)
        {
            customersRepository.LinkAadhaar(DematId, AadhaarNumber);
            TempData["SuccessMessage"] = "Aadhaar linked successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInvestment(CustomerDashboardViewModel model)
        {
            var accountNumber = model.NwAccount?.AccountNumber;

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                ModelState.AddModelError("", "Account Number is required.");
                return RedirectToAction("Index");
            }

            // 🔍 Fetch Customer
            var customer = customersRepository.GetCustomerByAccountNumber(accountNumber);
            if (customer == null)
            {
                ModelState.AddModelError("", "Customer not found.");
                return RedirectToAction("Index");
            }

            // 🔍 Fetch Demat for this Customer
            var demat = customersRepository.GetDematByCustomerId(customer.CustomerId);
            if (demat == null)
            {
                ModelState.AddModelError("", "No Demat account found for this customer.");
                return RedirectToAction("Index");
            }

            // ✅ Populate required fields
            var investment = model.NwInvestment;
            investment.CustomerId = customer.CustomerId;
            investment.DematId = demat.DematId;
            investment.CreatedAt = DateTime.Now;

            // 🗃️ Save to DB
            customersRepository.AddInvestment(investment);

            TempData["SuccessMessage"] = "Investment record created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetAccountSuggestions(string prefix)
        {
            var data = customersRepository.GetAccountSuggestions(prefix)
                .Select(a => new
                {
                    accountNumber = a.AccountNumber,
                    name = a.Customer.FirstName + " " + a.Customer.LastName
                }).ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAccountDetails(string accountNumber)
        {
            var account = customersRepository.GetAccountDetails(accountNumber);
            if (account == null) return Json(null);

            return Json(new
            {
                fullName = account.Customer.FirstName + " " + account.Customer.LastName,
                email = account.Customer.Email,
                balance = account.Balance,
                currency = account.Currency
            });
        }
    }
}
