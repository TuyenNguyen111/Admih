using FinalProject.Data;

namespace FinalProject.ViewModels
{
    public class UploadRoomPostVM
    {
        public IEnumerable<RoomTypeMenuVM> RoomTypes { get; set; }
        public UpdatePersonalInformationVM Users { get; set; }
        public List<int?> SelectedUtilities { get; set; } = new List<int?>();
        public List<UtilityVM> Utility { get; set; } = new List<UtilityVM>();
        public List<IFormFile> RoomImages { get; set; } = new List<IFormFile>();
        public RoomPostContentVM RoomPostContentVM { get; set; }
        public List<string> ImagesFromRoom { get; set; } = new List<string>();
    }
}
