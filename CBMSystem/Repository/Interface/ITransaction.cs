using CBMSystem.Models;

namespace CBMSystem.Repository.Interface
{
    public interface ITransaction
    {
        void Add(Transaction transaction);
        List<Transaction> GetRecent(int count);
        Transaction GetById(int id);
        List<Transaction> GetByAccount(string accountNumber);
        List<string> GetAccountNumbers();
    }
}
