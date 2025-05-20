using CBMSystem.Models;
using CBMSystem.ViewModels;

namespace CBMSystem.Repository.Interface
{
    public interface ICustomers 
    {
        void AddCustomer(CustomerDashboardViewModel customer, string? createdBy);
        CustomerAccount GetCustomerById(long id);
        IEnumerable<CustomerAccount> GetAllCustomers();
        int GetTotalCustomers();
        void UpdateCustomer(CustomerAccount customer);
        void DeleteCustomer(int id);
        void CreateDematAccount(DematAccount account);
        void LinkAadhaar(int dematId, string aadhaarNumber);
        void AddInvestment(Investment investment);
        //CustomerAccount GetCustomerById(int id);
        CustomerAccount GetCustomerByAccountNumber(string accountNumber);
        DematAccount GetDematByCustomerId(long customerId);

        //AccountNumberGETALLDETAILS
        IEnumerable<Account> GetAccountSuggestions(string prefix);
        Account? GetAccountDetails(string accountNumber);

        void UpdateCustomerImages(long customerId, string profilePath, string signaturePath);
        object GetAccountWithTransactions(string accountNumber);

        Account? GetAccountByCustomerId(long customerId);
        //List<Transaction> GetTransactionsByAccountNumber(string accountNumber);

        List<Transaction> GetTransactionsByStoredProcedure(long customerId, int months);
        //object GetTransactionsByStoredProcedure(long customerId, int months);
    }
}

