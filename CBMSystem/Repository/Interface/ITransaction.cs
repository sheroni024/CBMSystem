using CBMSystem.Models;

namespace CBMSystem.Repository.Interface
{
    public interface ITransaction
    {
        void Add(Transaction transaction);
        List<Transaction> GetRecent(int count);
        Transaction GetById(int id);
        void Delete(long id);
        List<Transaction> GetByAccount(string accountNumber);
        List<string> GetAccountNumbers();
        int GetAllTrans();
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetAllByUser(string email);
    }
}
