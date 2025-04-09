using CBMSystem.Models;
using CBMSystem.Repository;
using CBMSystem.Repository.Interface;
using CBMSystem.Repository.Service;
using CBMSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CBMSystem.Controllers
{
    public class CustomerAccountsController : Controller
    {
        private readonly ICustomers customersRepository;
        public CustomerAccountsController(ICustomers customersRepository)
        {
            this.customersRepository = customersRepository;
        }


        // Display all customers
        [HttpGet("all")]
        public IActionResult GetAllCustomers()
        {
            var customerList = customersRepository.GetAllCustomers();
            return View(customerList); // Ensure you have a "GetAllCustomers.cshtml" view
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
            return View(customer); // Ensure you have a "GetCustomerById.cshtml" view
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var viewModel = new CustomerDashboardViewModel();
            return View();
        }

        [HttpPost]
        public ActionResult Index(CustomerDashboardViewModel model)
        {
            if (model!=null)
            {
                try
                {
                    // Generate 11-digit AccountNumber
                    string generatedAccountNumber = new Random().Next(100000000, int.MaxValue).ToString().PadLeft(11, '0');

                    // Assign to both tables
                    model.NwCustomer.AccountNumber = generatedAccountNumber;
                    model.NwAccount.AccountNumber = generatedAccountNumber;

                    // Save via Repository
                    customersRepository.AddCustomer(model);

                    TempData["SuccessMessage"] = "Customer registered successfully!";
                    return RedirectToAction("Index");

                    //customersRepository.AddCustomer(model);
                    //TempData["SuccessMessage"] = "Customer registered successfully!";
                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
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
            if (ModelState.IsValid)
            {
                customersRepository.UpdateCustomer(model);
                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("GetAllCustomers");
            }
            return View(model);
        }

        [HttpGet("delete/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            customersRepository.DeleteCustomer(id);
            TempData["SuccessMessage"] = "Customer deleted successfully!";
            return RedirectToAction("GetAllCustomers");
        }

        //DematAccount 
        [HttpPost]
        public IActionResult CreateDemat(CustomerDashboardViewModel model)
        {
            if (model != null)
            {
                customersRepository.CreateDematAccount(model.NwDematAccount);
                TempData["SuccessMessage"] = "Demat Account created successfully!";
                return RedirectToAction("Index");
            }
            // Re-populate other properties if needed
            model.NwCustomer = customersRepository.GetCustomerById(model.NwDematAccount.CustomerId);
            return View("Index", model);
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
            if (model != null)
            {
                model.NwInvestment.CreatedAt = DateTime.Now;
                customersRepository.AddInvestment(model.NwInvestment);
                return RedirectToAction("Index", model); // Or wherever you want
            }

            return View(model);
        }
    }
}
