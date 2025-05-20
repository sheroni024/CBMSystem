using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using CBMSystem.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public void AddCustomer(CustomerDashboardViewModel model, string? createdBy)
        {
            if (model.NwCustomer == null || model.NwAccount == null)
                throw new ArgumentException("Customer or Account information is missing.");
            // Save Customer
            var customer = model.NwCustomer;

            //// Handle Profile Image
            //if (model.ProfileImageFile == null && model.ProfileImageFile.Length > 0)
            //{
            //    var ms = new MemoryStream();
            //    model.ProfileImageFile.CopyTo(ms);
            //    customer.ProfileImage = ms.ToArray();
            //}

            //// Handle Signature Image
            //if (model.SignatureImageFile == null && model.SignatureImageFile.Length > 0)
            //{
            //    var ms = new MemoryStream();
            //    model.SignatureImageFile.CopyTo(ms);
            //    customer.SignatureImage = ms.ToArray();
            //}

            customer.CreatedBy = createdBy;
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
                account.CreatedBy = createdBy;

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

        public void UpdateCustomerImages(long customerId, string profilePath, string signaturePath)
        {
            var customer = _context.CustomerAccounts.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                customer.ProfileImage = profilePath;
                customer.SignatureImage = signaturePath;
                _context.SaveChanges();
            }
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

        public CustomerAccount GetCustomerByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account Number cannot be null or empty");

            return _context.Accounts
                .Where(a => a.AccountNumber == accountNumber)
                .Select(a => a.Customer)
                .FirstOrDefault();
        }

        public void CreateDematAccount(DematAccount dematAccount)
        {
            //if (string.IsNullOrWhiteSpace(dematAccount.AccountNumber))
            //    throw new ArgumentException("Account Number cannot be null or empty");

            dematAccount.CreatedAt = DateTime.Now;
            dematAccount.IsActive = true;
            dematAccount.IsDelete = false;

            _context.DematAccounts.Add(dematAccount);
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
            if (string.IsNullOrWhiteSpace(investment.InvestmentName))
                throw new ArgumentException("Investment Name cannot be null or empty");

            investment.CreatedAt = DateTime.Now;
            _context.Investments.Add(investment);
            _context.SaveChanges();
        }

        public DematAccount? GetDematByCustomerId(long customerId)
        {
            return _context.DematAccounts.FirstOrDefault(d => d.CustomerId == customerId && d.IsActive == true);
        }

        //AccountNumberGETALLDETAILS
        public IEnumerable<Account> GetAccountSuggestions(string prefix)
        {
            return _context.Accounts
                .Where(a => a.AccountNumber.StartsWith(prefix) && (a.IsDelete == null || a.IsDelete == false))
                .Include(a => a.Customer)
                .Take(10)
                .ToList();
        }

        public Account? GetAccountDetails(string accountNumber)
        {
            return _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefault(a => a.AccountNumber == accountNumber && (a.IsDelete == null || a.IsDelete == false));
        }

        public object GetAccountWithTransactions(string accountNumber)
        {
            throw new NotImplementedException();
        }

        // Get customer account by customer ID
        public Account? GetAccountByCustomerId(long customerId)
        {
            return _context.Accounts
                .FirstOrDefault(a => a.CustomerId == customerId && a.IsDelete != true);
        }


        //public Account? GetAccountByCustomerId(long customerId)
        //{
        //    return _context.Accounts
        //        .FirstOrDefault(a => a.AccountNumber == Convert.ToString( customerId) && a.IsDelete != true);
        //}

        // Get transactions by account number
        //public List<Transaction> GetTransactionsByAccountNumber(string accountNumber)
        //{
        //    return _context.Transactions
        //        .Where(t => t.AccountNumber == accountNumber)
        //        .OrderByDescending(t => t.TransactionDate)
        //        .ToList();
        //}

        // Get transactions by stored procedure
        public List<Transaction> GetTransactionsByStoredProcedure(long customerId, int months)
        {
            return _context.Transactions
                .FromSqlRaw("EXEC dbo.GetCustomerTransactionsByMonths @p0, @p1", customerId, months)
                .ToList();
        }

        //public List<Transaction> GetTransactionsByStoredProcedure(string accountNumber, int months)
        //{
        //    throw new NotImplementedException();
        //}

        //object ICustomers.GetTransactionsByStoredProcedure(long customerId, int months)
        //{
        //    return GetTransactionsByStoredProcedure(customerId, months);
        //}

        //public List<Transaction> GetTransactionsByStoredProcedure(string accountNumber, int months)
        //{
        //    return _context.Transactions
        //        .FromSqlRaw("EXEC GetTransactionsByAccountAndMonths @AccountNumber = {0}, @Months = {1}", accountNumber, months)
        //        .ToList();
        //}

    }
}
