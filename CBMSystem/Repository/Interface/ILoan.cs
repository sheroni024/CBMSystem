using CBMSystem.Models;

namespace CBMSystem.Repository.Interface
{
    public interface ILoan
    {
        IEnumerable<Loan> GetAll();
        Loan GetById(long id);
        void Add(Loan loan);
        void Update(Loan loan);
        void Delete(long id);

        List<Loan> GetByAccount(string accountNumber);
        List<string> GetAccountNumbers();
        int GetAllLoan();
        IEnumerable<Loan> GetAllByUser(string email);
    }
}
