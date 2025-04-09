using Microsoft.AspNetCore.Authentication.Cookies;
//using JWTAuthenticationForProduct.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CBMSystem.Repository.Interface;
using CBMSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CBMSystem.Models;

namespace CBMSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _user;
        private readonly IConfiguration _config;

        public AccountController(IUser user, IConfiguration config)
        {
            _user = user;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(User login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private User AuthenticateUser(User login)
        {
            User user = null;
            if (login.Email.ToLower() == "manideep")
            {
                user = new User { 
                    
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

        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Login(LoginViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = _user.GetUserByEmailAndPassword(model.Email, model.Password);

        //    if (user != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ModelState.AddModelError("", "Invalid email or password.");
        //    return View(model);
        //}
        ////public async Task<IActionResult> Login(string email, string password)
        ////{
        ////    var user = _user.GetUserByEmailAndPassword(email, password);

        ////    if (user != null)
        ////    {
        ////        HttpContext.Session.SetString("UserEmail", user.Email);
        ////        //HttpContext.Session.SetString("UserName", user.Name);
        ////        return RedirectToAction("Index", "Home");
        ////    }

        ////    ViewBag.Error = "Invalid email or password.";
        ////    return View();
        ////}

        //[HttpPost]
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear(); // Clear session on logout
        //    return RedirectToAction("Index", "Home");
        //}

        //public IActionResult Register()
        //{
        //    return RedirectToAction("Register", "User");
        //}

        //[HttpGet]
        //public IActionResult TestSession()
        //{
        //    HttpContext.Session.SetString("Test", "Session Working!");
        //    return Content(HttpContext.Session.GetString("Test"));
        //}
    }
}
