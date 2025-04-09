using CBMSystem.Models;

namespace CBMSystem.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public CustomerDashboardViewModel()
        {
            NwCustomer = new CustomerAccount();
            NwDematAccount = new DematAccount();
            NwAccount = new Account();
            NwInvestment = new Investment();
        }

        public CustomerAccount NwCustomer { get; set; }
        public DematAccount NwDematAccount { get; set; }
        public Account NwAccount { get; set; }
        public Investment NwInvestment { get; set; }
    }
}
