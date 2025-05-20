using CBMSystem.Models;
using CBMSystem.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CBMSystem.Repository.Service
{
    public class UserRepository : IUser
    {
        private readonly CbmsContext context;
        private readonly IConfiguration _config;
        public UserRepository(CbmsContext context, IConfiguration config)
        {
            this.context = context;
            _config = config;
        }

        public void Register(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Password)) throw new ArgumentException("Password cannot be empty.", nameof(user.Password));

            user.CreatedDate = DateTime.Now;
            user.Token = "sheroni";
            user.IsActive = true;
            user.IsDelete = false;
            user.Password = HashPassword(user.Password);

            // **Hash Password Before Saving**

            context.Add(user);
            context.SaveChanges();
            Console.WriteLine($"User registered: {user.UserId}, Password Hash: {user.Password}");
        }

        public User Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Login attempt failed: Empty email or password.");
                return null;
            }
            password = HashPassword(password);
            var user = context.Users.FirstOrDefault(u => u.Email == email &&
                                       u.Password == password &&
                                       u.IsActive == true &&
                                       u.IsDelete == false);
            if (user == null)
            {
                Console.WriteLine($"No user found for email/username: {email}");
            }
            //Console.WriteLine($"User found: {user.UserId}, Stored Password Hash: {user.Password}");
            //Console.WriteLine($"Input Password Hash: {HashPassword(password)}");
            return user;
        }

        //private User AuthenticateUser(User login)
        //{
        //    User user = null;
        //    if (login.Email.ToLower() == "manideep")
        //    {
        //        user = new User
        //        {
        //            UserName = login.UserName,
        //            Email = login.Email
        //        };
        //    }
        //    return user;
        //}

        //private object GenerateJSONWebToken(User userInfo)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
        //        new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
        //        //new Claim("CreatedDate", userInfo.CreatedDate.ToString("yyyy-MM-dd")),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //        _config["Jwt:Issuer"],
        //        claims,
        //        expires: DateTime.Now.AddMinutes(120),
        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await context.Users.ToListAsync();
        }

        public User GetUserById(int userId)
        {
            return context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public User GetUserByEmailAndPassword(string email, string password)
        {
            return context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        }

        public User GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email && u.IsDelete != true);
        }

        public void UpdateUser(User user)
        {
            var existingUSer = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUSer != null)
            {
                existingUSer.FirstName = user.FirstName;
                existingUSer.LastName = user.LastName;
                existingUSer.Email = user.Email;

                if (!string.IsNullOrEmpty(user.Password) && !VerifyPassword(user.Password, existingUSer.Password)) 
                {
                    existingUSer.Password = HashPassword(user.Password);
                }

                context.Users.Update(existingUSer);
                context.SaveChanges();
            }
        }

        private static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHashedPassword))
                return false;

            string hashedInput = HashPassword(inputPassword);
            return hashedInput.Equals(storedHashedPassword, StringComparison.Ordinal);
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //Compute hash from password
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                //Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                //for(int i = 0; i < bytes.Length; i++)
                byte[] array = bytes;
                foreach (byte b in array)
                {
                    //Converted to its hexadecimal representation using the ToString("x2")
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public List<string> GetRecentActivity(string userEmail)
        {
            var recentTransactions = context.Transactions
                .Where(t => t.CreatedBy == userEmail)
                .OrderByDescending(t => t.TransactionDate)
                .Take(5)
                .Select(t => new
                {
                    Message = $"[Transaction] ₹{t.Amount:N2} on {t.TransactionDate:dd MMM yyyy} for {t.AccountNumber}",
                    Date = t.TransactionDate
                })
                .ToList();

            var recentLoans = context.Loans
                .Where(l => l.CreatedBy == userEmail)
                .OrderByDescending(l => l.AppliedDate)
                .Take(5)
                .Select(l => new
                {
                    Message = $"[Loan] ₹{l.LoanAmount:N2} ({l.LoanType}) applied on {l.AppliedDate:dd MMM yyyy}",
                    Date = l.AppliedDate
                })
                .ToList();

            var recentAccounts = context.CustomerAccounts
                .Where(c => c.CreatedBy == userEmail)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new
                {
                    Message = $"[Account] Created for {c.FirstName} {c.LastName} on {c.CreatedAt:dd MMM yyyy}",
                    Date = c.CreatedAt
                })
                .ToList();

            var recentDemats = context.DematAccounts
                .Where(d => d.CreatedBy == userEmail)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .Select(d => new
                {
                    Message = $"[Demat] Account #{d.AccountNumber} with {d.BrokerName} created on {d.CreatedAt:dd MMM yyyy}",
                    Date = d.CreatedAt ?? DateTime.MinValue
                })
                .ToList();

            var recentInvestments = context.Investments
                .Where(i => i.CreatedBy == userEmail)
                .OrderByDescending(i => i.InvestmentDate)
                .Take(5)
                .Select(i => new
                {
                    Message = $"[Investment] ₹{i.InvestmentAmount:N2} in {i.InvestmentName} on {i.InvestmentDate:dd MMM yyyy}",
                    Date = i.InvestmentDate ?? DateTime.MinValue
                })
                .ToList();

            
            // Combine all messages into one list of (message, date)
            var recentActivities = new List<(string Message, DateTime Date)>();

            recentActivities.AddRange(recentTransactions.Select(t => (t.Message, t.Date)));
            recentActivities.AddRange(recentLoans.Select(l => (l.Message, l.Date)));
            recentActivities.AddRange(recentAccounts.Select(a => (a.Message, a.Date)));
            recentActivities.AddRange(recentDemats.Select(d => (d.Message, d.Date)));
            recentActivities.AddRange(recentInvestments.Select(i => (i.Message, i.Date)));
            // Example: Add custom report generation message
            var generatedReportMessage = $"[Report] Report generated in PDF/TXT format for Account ID: {userEmail} on {DateTime.Now:dd MMM yyyy hh:mm tt}";
            recentActivities.Add((generatedReportMessage, DateTime.Now));

            // Example: Add profile update message
            var profileUpdateMessage = $"[Profile] User profile updated on {DateTime.Now:dd MMM yyyy hh:mm tt}";
            recentActivities.Add((profileUpdateMessage, DateTime.Now));

            // Sort and return top 10 as plain messages
            return recentActivities
                .OrderByDescending(a => a.Date)
                .Take(10)
                .Select(a => a.Message)
                .ToList();
            
        }

        public void Update(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        //private User AuthenticateUser(User login)
        //{
        //    User? user = null;
        //    if (login.Email.ToLower() == "manideep")
        //    {
        //        user = new User
        //        {
        //            //UserName = login.UserName,
        //            Email = login.Email
        //        };
        //    }
        //    return user;
        //}

        public void UpdateUserToken(User user)
        {
            var dbUser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (dbUser != null)
            {
                dbUser.Token = user.Token;
                context.SaveChanges();
            }
        }

        public User GetUserByToken(string sessionToken)
        {
            throw new NotImplementedException();
        }


        //    public List<string> GetRecentActivity(string userEmail)
        //    {
        //        var recentActivities = new List<string>();

        //        var recentTransactions = context.Transactions
        //            .Where(t => t.CreatedBy == userEmail)
        //            .OrderByDescending(t => t.TransactionDate)
        //            .Take(5)
        //            .Select(t => $"Transaction of ₹{t.Amount:N2} on {t.TransactionDate:dd MMM yyyy} for {t.AccountNumber}")
        //            .ToList();

        //        var recentLoans = context.Loans
        //            .Where(l => l.CreatedBy == userEmail) // assuming Loan also has CreatedBy
        //            .OrderByDescending(l => l.AppliedDate)
        //            .Take(5)
        //            .Select(l => $"Loan applied for ₹{l.LoanAmount:N2} ({l.LoanType}) on {l.AppliedDate:dd MMM yyyy}")
        //            .ToList();

        //        var recentAccounts = context.CustomerAccounts
        //            .Where(c => c.CreatedBy == userEmail) // or c.CreatedBy if using that
        //            .OrderByDescending(c => c.CreatedAt)
        //            .Take(5)
        //            .Select(c => $"Account created for {c.FirstName} {c.LastName} on {c.CreatedAt:dd MMM yyyy}")
        //            .ToList();

        //        // Demat Accounts
        //var recentDemats = context.DematAccounts
        //    .Where(d => d.CreatedBy == userEmail)
        //    .OrderByDescending(d => d.CreatedAt)
        //    .Take(5)
        //    .Select(d => new
        //    {
        //        Message = $"[Demat] Account #{d.AccountNumber} with {d.BrokerName} created on {d.CreatedAt:dd MMM yyyy}",
        //        Date = d.CreatedAt ?? DateTime.MinValue
        //    })
        //.ToList();

        //        // Investments
        //        var recentInvestments = context.Investments
        //    .Where(i => i.CreatedBy == userEmail)
        //    .OrderByDescending(i => i.InvestmentDate)
        //    .Take(5)
        //    .Select(i => new
        //    {
        //        Message = $"[Investment] ₹{i.InvestmentAmount:N2} in {i.InvestmentName} on {i.InvestmentDate:dd MMM yyyy}",
        //        Date = i.InvestmentDate ?? DateTime.MinValue
        //    })
        //.ToList();

        //        recentActivities.AddRange(recentTransactions);
        //        recentActivities.AddRange(recentLoans);
        //        recentActivities.AddRange(recentAccounts);
        //        recentActivities.AddRange(recentDemats.Select(d => (d.Message, d.Date)));
        //        recentActivities.AddRange(recentInvestments.Select(i => (i.Message, i.Date)));

        //        return recentActivities
        //.OrderByDescending(a => a.Date)
        //.Take(10)
        //.Select(a => a.Message)
        //.ToList();
        //    }


        //public List<string> GetRecentActivity()
        //{
        //    var recentActivities = new List<string>();

        //    // Recent Transactions
        //    var recentTransactions = context.Transactions
        //        .OrderByDescending(t => t.TransactionDate)
        //        .Take(5)
        //        .Select(t => $"Transaction of ₹{t.Amount:N2} on {t.TransactionDate:dd MMM yyyy} for {t.AccountNumber}")
        //        .ToList();

        //    // Recent Loans
        //    var recentLoans = context.Loans
        //        .OrderByDescending(l => l.AppliedDate)
        //        .Take(5)
        //        .Select(l => $"Loan applied for ₹{l.LoanAmount:N2} ({l.LoanType}) on {l.AppliedDate:dd MMM yyyy}")
        //        .ToList();

        //    // Recent Customer Accounts
        //    var recentAccounts = context.CustomerAccounts
        //        .OrderByDescending(c => c.CreatedAt)
        //        .Take(5)
        //        .Select(c => $"Account created for {c.FirstName} on {c.CreatedAt:dd MMM yyyy}")
        //        .ToList();

        //    recentActivities.AddRange(recentTransactions);
        //    recentActivities.AddRange(recentLoans);
        //    recentActivities.AddRange(recentAccounts);

        //    // Return top 10 activities
        //    return recentActivities
        //        .OrderByDescending(a => a) // You could replace with actual timestamp if available
        //        .Take(10)
        //        .ToList();
        //}

        //public void UpdateProfile(User user)
        //{
        //    var existingUser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
        //    if (existingUser != null) 
        //    {
        //        existingUser.FirstName = user.FirstName;
        //        existingUser.LastName = user.LastName;
        //        existingUser.DateOfBirth = user.DateOfBirth;
        //        existingUser.Gender = user.Gender;
        //        existingUser.Email = user.Email;
        //        existingUser.PhoneNumber = user.PhoneNumber;

        //        if (!string.IsNullOrEmpty(user.Password) && !VerifyPassword(user.Password, existingUser.Password))
        //        {
        //            existingUser.Password = HashPassword(user.Password);
        //        }

        //        existingUser.Address = user.Address;
        //        existingUser.IdproofType = user.IdproofType;
        //        existingUser.IdproofNumber = user.IdproofNumber;
        //        existingUser.Kycstatus = user.Kycstatus;
        //        existingUser.UserType = user.UserType;
        //        existingUser.RoleId = user.RoleId;
        //        context.SaveChanges();
        //    }

        //    //context.Users.Update(user);
        //}


        //public static List<string> GetIdProofTypes()
        //{
        //    return new List<string> { "Passport", "Driver’s License", "Aadhar Card", "Voter ID" };
        //}

    }
}
