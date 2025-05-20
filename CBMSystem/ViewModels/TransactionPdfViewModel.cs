using CBMSystem.Models;

namespace CBMSystem.ViewModels
{
    public class TransactionPdfViewModel
    {
        public string AccountNumber { get; set; }
        public long CustomerId { get; set; }
        public int Months { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
