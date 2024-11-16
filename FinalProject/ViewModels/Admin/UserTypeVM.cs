using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.Admin
{
    public class UserTypeVM
    {
        public int UserTypeId { get; set; }
        [Required(ErrorMessage = "Tên nhóm người dùng là bắt buộc.")]
        public string UserTypeName { get; set; }
    }
}
