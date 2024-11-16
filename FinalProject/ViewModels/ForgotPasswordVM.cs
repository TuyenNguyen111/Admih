using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid emaild address")]
        public string Email { get; set; }
    }
}
