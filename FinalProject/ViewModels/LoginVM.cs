using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "* Username is required.")]
        [StringLength(50, ErrorMessage = "* Username must be between 3 and 50 characters.", MinimumLength = 3)]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain uppercase letters, lowercase letters, digits, and special characters.")]
        public string password { get; set; }
    }
}
