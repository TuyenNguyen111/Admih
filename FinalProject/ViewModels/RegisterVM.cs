using FinalProject.Validation;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class RegisterVM
    {

        [Required(ErrorMessage = "* Username is required.")]
        [StringLength(50, ErrorMessage = "* Username must be between 3 and 50 characters.", MinimumLength = 3)]
        public string username { get; set; }


        [Required(ErrorMessage = "* Password is required.")]
        [StringLength(100, ErrorMessage = "* Password must be between 6 and 100 characters.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "* Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string password { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        [AgeRestriction(18)]
        public DateTime dob { get; set; }


        [Required(ErrorMessage = "* Email is required.")]
        [EmailAddress(ErrorMessage = "* Invalid email format.")]
        public string email { get; set; }

        public bool gender { get; set; } = false;
        [Required(ErrorMessage = "Please select an account type.")]
        public int accountType { get; set; }

        public RegisterVM() { }

        public RegisterVM(string username, string password, DateTime dob, string email, bool gender, int accountType)
        {
            this.username = username;
            this.password = password;
            this.dob = dob;
            this.email = email;
            this.gender = gender;
            this.accountType = accountType;
        }
    }
}
