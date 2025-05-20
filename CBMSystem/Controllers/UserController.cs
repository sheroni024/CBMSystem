using CBMSystem.Repository.Interface;
using CBMSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using CBMSystem.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using CBMSystem.ViewModels;
using CBMSystem.Repository.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using NuGet.Common;
using CBMSystem.Filters;


namespace CBMSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _userRepository;
        private readonly ICustomers customersRepository;
        private readonly ITransaction transactionRepository;
        private readonly ILoan loanRepository;
        private readonly ISearch _navSearch;
        private readonly IConfiguration _config;
        private readonly EmailService emailService;

        public UserController(IUser userRepository, ICustomers customersRepository, ITransaction transactionRepository,  ILoan loanRepository, ISearch _navSearch, IConfiguration config, EmailService emailService)
        {
            _userRepository = userRepository;
            this.customersRepository = customersRepository;
            this.transactionRepository = transactionRepository;
            this.loanRepository = loanRepository;
            this._navSearch = _navSearch;
            _config = config;
            this.emailService = emailService;
        }

        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityUser> _roleManager;
        //private readonly IConfiguration _configuration;

        //public UserController(IUser userRepository, UserManager<IdentityUser> userManager, RoleManager<IdentityUser> roleManager, IConfiguration configuration)
        //{
        //    _userRepository = userRepository;
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //    _configuration = configuration;
        //}

        public IActionResult Index()
        {

            string st = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new User();
            //ViewData["IdProofTypes"] = User.GetIdProofTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (user == null)
            {
                return View(user);
            }
            _userRepository.Register(user);
            TempData["SuccessM"] = "Registration Successful!";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            //login.Email = email;
            //login.Password = password;
            //IActionResult response = Unauthorized();
            //var user = AuthenticateUser(login);
            //if (user != null)
            //{
            //    var tokenString = GenerateJSONWebToken(user);
            //    response = Ok(new { token = tokenString });
            //}
            //return response;

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage("Please enter both email and password.");
                return View();
            }

            var users = _userRepository.Login(model.Email, model.Password);
            if (users != null)
            {
                var tokenString = GenerateJSONWebToken(users);
                users.Token = (string)tokenString;

                //HttpContext.Session.SetString("AuthToken", (string)tokenString);
                
                // ✅ Save token in DB (e.g., for session tracking or logout)
                _userRepository.UpdateUserToken(users);

                HttpContext.Session.SetInt32("UserId", (int)users.UserId);
                HttpContext.Session.SetString("UserEmail", users.Email);
                HttpContext.Session.SetString("Token", (string)tokenString);

                // ✅ Store in Cookie
                Response.Cookies.Append("AuthToken", (string)tokenString , new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // ✉️ Send login success email
                string subject = "Login Successful!";
                string body = $"Hello {users.FirstName} {users.LastName},<br/><br/>You have successfully logged in to your account.<br/><br/>If this wasn't you, please change your password immediately.<br/><br/>Thanks,<br/>Cashless Banking Team";

                emailService.SendEmail(users.Email, subject, body);

                //// ✅ Generate Token
                //var tokenString = GenerateJSONWebToken(users);
                //HttpContext.Session.SetString("AuthToken", (string)tokenString);

                TempData["LoginSuccess"] = "Logged in Successfully!";
                return RedirectToAction("Dashboard");
            }
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        private User AuthenticateUser(User login)
        {
            User? user = null;
            if (login.Email.ToLower() == "manideep")
            {
                user = new User
                {
                    //UserName = login.UserName,
                    Email = login.Email
                };
            }
            return user;
        }

        private object GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("UserId", userInfo.UserId.ToString()),
                //new Claim("CreatedDate", userInfo.CreatedDate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public IActionResult Login(LoginViewModel model)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    return View(model);
        //    //}
        //    string hashedPassword = _userRepository.HashPassword(model.Password);
        //    User user = _userRepository.GetUserByEmailAndPassword(model.Email, hashedPassword);

        //    //if (user.Password != model.Password)
        //    //{
        //    //    model.ErrorMessage = "Incorrect password.";
        //    //    return View(model);
        //    //}

        //    if (user != null && user.Password == hashedPassword)
        //    {
        //        HttpContext.Session.SetInt32("UserId", (int)user.UserId);
        //        HttpContext.Session.SetString("UserEmail", user.Email);
        //        HttpContext.Session.SetString("UserName", user.FirstName);
        //        TempData["LoginSuccess"] = "Logged in successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    ModelState.AddModelError("", "Invalid email or password.");
        //    return View();
        //}

        //[Authorize]
        [AuthorizeToken]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                return RedirectToAction("login", "user");
            }

            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            //{
            //    return RedirectToAction("Login");
            //}

            var user = _userRepository.GetUserById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Set employee name for the dashboard
            ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
            ViewBag.TotalCustomers = customersRepository.GetTotalCustomers();
            ViewBag.TransactionsToday = transactionRepository.GetAllTrans();

            ViewBag.PendingLoans = loanRepository.GetAllLoan();/*.Count(l => l.Status?.StatusName == "Pending");*/
            ViewBag.TotalDeposits = "$5,500,000";
            ViewBag.RevenueThisMonth = "$85,000";
            ViewBag.TotalDeposits = transactionRepository.GetAll()
            .Where(t => t.TransactionType == "Deposit")
            .Sum(t => t.Amount);
            ViewBag.RevenueThisMonth = transactionRepository.GetAll()
                .Where(t => t.TransactionDate.Month == DateTime.Now.Month &&
                            t.TransactionDate.Year == DateTime.Now.Year)
                .Sum(t => t.Amount);
            //ViewBag.RecentActivity = new List<string>
            //    {
            //        "Processed Loan Approval",
            //        "Transferred $1,000 to Client",
            //        "Generated Financial Report"
            //     };
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.RecentActivity = _userRepository.GetRecentActivity(userEmail); // updated method


            return View();  //No need to pass an anonymous object
        }

        // GET: /User/VerifyOtp
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        // POST: /User/VerifyOtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(string otp)
        {
            if (otp == TempData["OTP"]?.ToString())
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.ErrorMessage = "Invalid OTP.";
            return View();
        }

        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            var url = _navSearch.GetRedirectUrlForSearch(searchTerm);
            if (string.IsNullOrEmpty(url))
            {
                // Optionally, redirect to a NotFound or fallback view
                TempData["SearchMessage"] = "No matching page found.";
                return RedirectToAction("Index", "Home");
            }

            return Redirect(url);
        }


        [HttpGet]
        public JsonResult GetSearchSuggestions(string term)
        {
            var isAuthenticated = HttpContext.Session.GetString("UserEmail") != null;
            var allTerms = new List<string> { "AddCustomer", "DeleteTransaction", "Loan", "CustomerIndex", "TransactionIndex" };

            var suggestions = allTerms
                .Where(t => t.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Json(new { isAuthenticated, suggestions });
        }


        private bool IsValidUser(string username, string password)
        {
            // Replace with your own logic to validate user
            return username == "admin" && password == "password";
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session on logout
            return RedirectToAction("Login", "User");

            var sessionToken = HttpContext.Session.GetString("Token");
            if (sessionToken != null)
            {
                var user = _userRepository.GetUserByToken(sessionToken);
                if (user != null)
                {
                    user.Token = null;
                    _userRepository.UpdateUserToken(user);
                }
            }

            HttpContext.Session.Clear();
            if (Request.Cookies.ContainsKey("AuthToken"))
            {
                Response.Cookies.Delete("AuthToken");
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult TestSession()
        {
            HttpContext.Session.SetString("Test", "Session Working!");
            return Content(HttpContext.Session.GetString("Test"));
            //return View(TestSession);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Please enter Valid Email!";
                return View();
            }
            User user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                ViewBag.ErrorMessage= "Email not found.";
                return View();
            }
            TempData["UserEmail"] = email;
            return RedirectToAction("SetNewPassword");
        }
        //public IActionResult ForgotPassword(ChangePasswordVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    User user = _user.GetUserByEmail(model.Email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("", "Email not found.");
        //        return View(model);
        //    }
        //    //return Content("Form Submitted Successfully");
        //    // Redirect to Set New Password
        //    return RedirectToAction("SetNewPassword", new
        //    {
        //        email = model.Email
        //    });
        //}

        //public IActionResult SetNewPassword(string email)
        //{
        //    var user = _user.GetUserByEmail(email);
        //    if (user == null)
        //    {
        //        return RedirectToAction("ForgotPassword");
        //    }
        //    var model = new ChangePasswordVM { Email = email };
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult SetNewPassword()
        {
            if (TempData["UserEmail"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            TempData.Keep("UserEmail");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetNewPassword(string newPassword, string confirmNewPassword)
        {
            string email = TempData["UserEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            {
                ViewBag.ErrorMessage = "Both password fields are required.";
                return View();
            }

            if (newPassword != confirmNewPassword)
            {
                ViewBag.ErrorMessage = "New Password and Confirm Password do not match.";
                return View();
            }

            var user = _userRepository.GetUserByEmail(email);
            if (user != null)
            {
                user.Password = newPassword;
                _userRepository.UpdateUser(user);

                // ✉️ Send success email
                string subject = "Password Reset Successfully";
                string body = $"Hello {user.FirstName} {user.LastName},<br/><br/>Your password has been successfully reset.<br/><br/>Thanks,<br/>Cashless Banking Team";

                emailService.SendEmail(user.Email, subject, body);

                TempData["SuccessM"] = "Password is Reset Successfully!";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }
            return View();
            // Update password
            //user.Password = _userRepository.HashPassword(model.NewPassword);
            //user.Password = model.NewPassword;
        }

        //SETTINGS
        public IActionResult Profile()
        {
            //var email = User.Identity?.Name;
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _userRepository.GetUserByEmail(userEmail!);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            //var email = User.Identity?.Name;
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _userRepository.GetUserByEmail(userEmail!);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email))
                {
                ModelState.AddModelError("Email", "Email is required.");
                return View(model);
            }

            _userRepository.Update(model);

            TempData["Success"] = "Profile Updated Successfully!";
            return RedirectToAction("Profile");

            //if (model == null || string.IsNullOrEmpty(model.Email))
            //    return View(model);

            //var existingUser = _userRepository.GetUserById(model.UserId);

            //if (existingUser == null)
            //    return NotFound();

            //// Only update the necessary fields
            //existingUser.FirstName = model.FirstName;
            //existingUser.LastName = model.LastName;
            //// Don't touch Email if it's not part of the form

            //_userRepository.Update(existingUser);

            //TempData["Success"] = "Profile Updated Successfully!";
            //return RedirectToAction("Profile");
        }
    }
    
}
