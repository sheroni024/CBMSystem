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


        // New properties for file uploads
        public IFormFile? ProfileImageFile { get; set; }
        public IFormFile? SignatureImageFile { get; set; }

        // Properties to store the paths once uploaded
        public string? ProfileImagePath { get; set; }
        public string? SignatureImagePath { get; set; }
    }
}
