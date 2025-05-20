using CBMSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CBMSystem.Repository.Interface
{
    public interface IUser
    {
        void Register(User user);
        User Login(string email, string password);
        void UpdateUser(User user);
        User GetUserById(int id);
        User GetUserByEmailAndPassword(string email, string password);
        User GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
        void Update(User user);
        List<string> GetRecentActivity(string userEmail);
        void UpdateUserToken(User user);
        User GetUserByToken(string sessionToken);
        //void RegisterUser(User user);
        //string HashPassword(string password);
    }
}
