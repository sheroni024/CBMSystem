using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using CBMSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace CBMSystem.Repository.Service
{
    public class CustomerRepository : ICustomers
    {
        private readonly CbmsContext _context;

        public CustomerRepository(CbmsContext context)
        {
            this._context = context;
        }

        public void AddCustomer(CustomerDashboardViewModel model)
        {
            //using (var context = new CbmsContext())
            //{
                // Save Customer
                var customer = model.NwCustomer;
                customer.CreatedAt = DateTime.Now;
                customer.IsActive = true;
                customer.IsDelete = false;

                _context.CustomerAccounts.Add(customer);
                _context.SaveChanges(); // Generates CustomerId

                // Now get the saved CustomerID (auto-generated)
                long customerId = customer.CustomerId;

                // Create Account linked to Customer
                var account = model.NwAccount;
                account.CustomerId = customerId;
                account.BranchCodeId = 1; // default/fake for now
                account.StatusId = 1;
                account.CreatedDate = DateTime.Now;
                account.IsActive = true;
                account.IsDelete = false;

            _context.Accounts.Add(account);

            //// Optionally save Demat too (if provided)
            //if (!string.IsNullOrEmpty(model.NwDematAccount?.BrokerName))
            //{
            //    var demat = model.NwDematAccount;
            //    demat.CustomerID = customerId;
            //    context.DematAccounts.Add(demat);
            //}

            _context.SaveChanges();

                //_context.CustomerAccounts.Add(customer);
                //_context.SaveChanges();
            //}
        }

        public CustomerAccount GetCustomerById(long id)
        {
            return _context.CustomerAccounts.Find(id) ?? throw new Exception("Customer not found");
        }

        public IEnumerable<CustomerAccount> GetAllCustomers()
        {
            return _context.CustomerAccounts.ToList();
        }
        public int GetTotalCustomers()
        {
            return _context.CustomerAccounts.Count();
        }

        public void UpdateCustomer(CustomerAccount customer)
        {
            var existingCustomer = _context.CustomerAccounts.Find(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Email = customer.Email;
                existingCustomer.MobileNumber = customer.MobileNumber;
                existingCustomer.PancardNumber = customer.PancardNumber;
                existingCustomer.AadhaarNumber = customer.AadhaarNumber;
                existingCustomer.Address = customer.Address;
                existingCustomer.DateOfBirth = customer.DateOfBirth;
                existingCustomer.Gender = customer.Gender;

                _context.SaveChanges();
            }
        }

        public void DeleteCustomer(int id)
        {
            var customer = _context.CustomerAccounts.Find(id);
            if (customer != null)
            {
                _context.CustomerAccounts.Remove(customer);
                _context.SaveChanges();
            }
        }

        public void CreateDematAccount(DematAccount account)
        {

            _context.DematAccounts.Add(account);
            _context.SaveChanges();
            
        }

        public void LinkAadhaar(int dematId, string aadhaarNumber)
        {
            var account = _context.DematAccounts.FirstOrDefault(d => d.CustomerId == dematId);
                if (account != null)
                {
                    account.AadhaarLinked = true;
                    _context.SaveChanges();
                }
        }

        public void AddInvestment(Investment investment)
        {
            _context.Investments.Add(investment);
            _context.SaveChanges();
        }
    }
}
