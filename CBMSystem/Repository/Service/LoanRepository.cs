using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace CBMSystem.Repository.Service
{
    public class LoanRepository : ILoan
    {
        private readonly CbmsContext _context;

        public LoanRepository(CbmsContext context)
        {
            _context = context;
        }

        public IEnumerable<Loan> GetAll()
        {
            return _context.Loans
                .Where(l => l.IsDelete == false || l.IsDelete == null)
                .Include(l => l.Status)
                //.Include(l => l.AccountNumber)
                .ToList();
        }

        public Loan GetById(long id)
        {
            return _context.Loans
                .Include(l => l.Status)
                //.Include(l => l.AccountNumber)
                .FirstOrDefault(l => l.LoanId == id);
        }

        public void Add(Loan loan)
        {
            loan.AppliedDate = DateTime.Now;
            loan.IsActive = true;
            loan.IsDelete = false;
            _context.Loans.Add(loan);
            _context.SaveChanges();
        }

        public void Update(Loan loan)
        {
            loan.UpdatedDate = DateTime.Now;
            //_context.Loans.Update(loan);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var loan = _context.Loans.Find(id);
            if (loan != null)
            {
                loan.IsDelete = true;
                _context.SaveChanges();
            }
        }

        public List<Loan> GetByAccount(string accountNumber)
        {
            return _context.Loans
                .Where(t => t.AccountNumber == accountNumber)
                .OrderByDescending(t => t.AppliedDate)
                .ToList();
        }

        public List<string> GetAccountNumbers()
        {
            return _context.Accounts
                .Select(a => a.AccountNumber)
                .ToList() ?? new List<string>();

        }
        public int GetAllLoan()
        {
            return _context.Loans.Count();
        }

        public IEnumerable<Loan> GetAllByUser(string email)
        {
            return _context.Loans
                .Where(l => l.CreatedBy == email)
                .ToList();
        }

    }
}
