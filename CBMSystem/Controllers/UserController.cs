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


namespace CBMSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _userRepository;
        private readonly ICustomers customersRepository;
        private readonly IConfiguration _config;

        public UserController(IUser userRepository, ICustomers customersRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            this.customersRepository = customersRepository;
            _config = config;
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

            string st =     HttpContext.Session.GetString("UserEmail");
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
        public IActionResult Register(User user)
        {
            if(!ModelState.IsValid)
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
        public IActionResult Login(string email, string password, User login)
        {
            login.Email = email;
            login.Password = password;
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            //return response;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage("Please enter both email and password.");
                return View();
            }

            var users = _userRepository.Login(email, password);
            if (users != null)
            {
                HttpContext.Session.SetInt32("UserId", (int)users.UserId);
                HttpContext.Session.SetString("UserEmail", users.Email);
                TempData["LoginSuccess"] = "Logged in successfully!";
                return RedirectToAction("Dashboard");
            }
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();


        }

        private User AuthenticateUser(User login)
        {
            User user = null;
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
                //new Claim("CreatedDate", userInfo.CreatedDate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
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
            ViewBag.TransactionsToday = 320;
            ViewBag.PendingLoans = 45;
            ViewBag.TotalDeposits = "$5,500,000";
            ViewBag.RevenueThisMonth = "$85,000";
            ViewBag.RecentActivity = new List<string>
                {
                    "Processed Loan Approval",
                    "Transferred $1,000 to Client",
                    "Generated Financial Report"
                 };

            return View();  //No need to pass an anonymous object
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

    }
}
