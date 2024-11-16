using FinalProject.Validation;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.Admin
{
    public class UserManagementVM
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Trường tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự.", MinimumLength = 3)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Trường họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Trường họ tên người dùng không vượt quá 100 ký tự")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Trường Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Trường số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Loại người dùng là bắt buộc.")]
        public int? UserTypeId { get; set; }
    
        public string? TypeName { get; set; }

        public bool IsValid { get; set; }
       [Required(ErrorMessage = "Trường ngày sinh là bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày không hợp lệ.")]
        [AgeRestriction(18, ErrorMessage = "Bạn phải từ 18 tuổi trở lên.")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string Password { get; set; } = string.Empty;
    }
}
