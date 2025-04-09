using System.ComponentModel.DataAnnotations;

namespace CBMSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is requied.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is requied.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }
    }
}
