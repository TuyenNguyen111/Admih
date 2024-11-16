using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels
{
    public class RoomPostContentVM
    {
        public int PostId { get; set; }
        public string RoomName { get; set; } = "";
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Mô tả không được quá 1000 ký tự.")]
        public string RoomDescription { get; set; }
        [Required(ErrorMessage = "Room price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Room price must be a positive number.")]
        public decimal RoomPrice { get; set; }
        [Required(ErrorMessage = "Diện tích là bắt buộc.")]
        [Range(1, 1000, ErrorMessage = "Diện tích phải từ 1m² đến 1000m².")]
        public decimal RoomSize { get; set; }
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string RoomAddress { get; set; }
        [Required(ErrorMessage = "Loại phòng là bắt buộc.")]
        public int RoomTypeId { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public int RoomCoordinateId { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
