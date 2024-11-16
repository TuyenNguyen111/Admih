using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class UpdatePasswordVM
    {
        [Required(ErrorMessage = "Current password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain uppercase letters, lowercase letters, digits, and special characters.")]
        public string currentPassword { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain uppercase letters, lowercase letters, digits, and special characters.")]
        public string newPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must be at least 6 characters long and contain uppercase letters, lowercase letters, digits, and special characters.")]
        [Compare("newPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string confirmPassword { get; set; }
        public UpdatePasswordVM() { }
        public UpdatePasswordVM(string currentPassword, string newPassword, string confirmPassword)
        {
            this.currentPassword = currentPassword;
            this.newPassword = newPassword;
            this.confirmPassword = confirmPassword;
        }
    }
}
