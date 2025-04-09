using System.ComponentModel.DataAnnotations;

namespace CBMSystem.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Email is requied.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is requied.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} character long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is requied.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
