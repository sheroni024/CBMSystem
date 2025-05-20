using CBMSystem.Models;
using CBMSystem.Repository.Interface;

namespace CBMSystem.Repository.Service
{
    public class    TransactionRepository : ITransaction
    {
        private readonly CbmsContext _context;
        public TransactionRepository(CbmsContext context)
        {
            _context = context;
        }

        public void Add(Transaction transaction)
        {
            transaction.TransactionDate = DateTime.Now;
            transaction.BranchCodeId = 1; // or fetch based on account if needed
            transaction.StatusId = 1;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public List<Transaction> GetRecent(int count)
        {
            return _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .ToList();
        }

        public void Delete(long id)
        {
            var loan = _context.Transactions.Find(id);
            //if (loan != null)
            //{
            //    loan.IsDelete = true;
            //    _context.SaveChanges();
            //}
        }

        public Transaction GetById(int id)
        {
            return _context.Transactions.FirstOrDefault(t => t.TransactionId == id);
        }

        public List<Transaction> GetByAccount(string accountNumber)
        {
            return _context.Transactions
                .Where(t => t.AccountNumber == accountNumber)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public List<string> GetAccountNumbers()
        {
            return _context.Accounts
                .Select(a => a.AccountNumber)
                .ToList() ?? new List<string>();

        }
        public int GetAllTrans()
        {
            return _context.Transactions.Count();
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public IEnumerable<Transaction> GetAllByUser(string email)
        {
            return _context.Transactions
                .Where(t => t.CreatedBy == email)
                .ToList();
        }
    }
}
