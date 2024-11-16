using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class UpdatePersonalInformationVM
    {
        [Required(ErrorMessage = "Fullname is required.")]
        [StringLength(100, ErrorMessage = "Fullname cannot exceed 100 characters.")]
        public string? fullname { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number format for Vietnam.")]
        public string? phone { get; set; }
        public bool gender { get; set; }

        [Url(ErrorMessage = "Invalid URL format for the image.")]
        public string? userimage { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2100", ErrorMessage = "Date of Birth must be between 01/01//1900 and 31/12/2100.")]
        public DateTime? dob { get; set; }

        public UpdatePersonalInformationVM() { }

        public UpdatePersonalInformationVM(string fullname, string email, string phone, bool gender, string userimage, DateTime dob)
        {
            this.fullname = fullname;
            this.email = email;
            this.phone = phone;
            this.gender = gender;
            this.userimage = userimage;
            this.dob = dob;
        }
    }
}
