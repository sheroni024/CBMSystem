using CBMSystem.Models;

namespace CBMSystem.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; } // "customer", "loan", "investment", etc.
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        // Include all your data models
        public CustomerAccount XCustomerAccount { get; set; }
        public DematAccount XDematAccount { get; set; }
        public Account XAccount { get; set; }
        public Investment XInvestment { get; set; }
        public Loan XLoan { get; set; }
        public Transaction XTransaction { get; set; }
        public FraudReport XFraudReport { get; set; }
    }
}
