using CBMSystem.Models;
using CBMSystem.ViewModels;

namespace CBMSystem.Repository.Interface
{
    public interface ICustomers 
    {
        void AddCustomer(CustomerDashboardViewModel customer);
        CustomerAccount GetCustomerById(long id);
        IEnumerable<CustomerAccount> GetAllCustomers();
        int GetTotalCustomers();
        void UpdateCustomer(CustomerAccount customer);
        void DeleteCustomer(int id);
        void CreateDematAccount(DematAccount account);
        void LinkAadhaar(int dematId, string aadhaarNumber);
        void AddInvestment(Investment investment);
        //CustomerAccount GetCustomerById(int id);

    }
}
