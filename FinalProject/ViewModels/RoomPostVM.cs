namespace FinalProject.ViewModels
{
    public class RoomPostVM
    {
        public int PostId { get; set; }
        public string RoomName { get; set; }
        public int Quantity { get; set; }
        public string RoomDescription { get; set; }
        public string RoomImage { get; set; }
        public decimal RoomPrice { get; set; }
        public decimal RoomSize { get; set; }
        public string RoomAddress { get; set; }
        public string RoomType { get; set; }
        public int UserId { get; set; }
    }
}
