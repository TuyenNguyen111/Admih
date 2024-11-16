namespace FinalProject.ViewModels
{
    public class RoomListVM
    {
        public List<RoomPostVM> Rooms { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
