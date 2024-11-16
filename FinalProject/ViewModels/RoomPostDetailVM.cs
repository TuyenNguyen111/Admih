namespace FinalProject.ViewModels
{
    public class RoomPostDetailVM
    {
        public int PostId { get; set; }
        public string RoomName { get; set; }
        public int Quantity { get; set; }
        public string RoomDescription { get; set; }
        public decimal RoomPrice { get; set; }
        public decimal RoomSize { get; set; }
        public string RoomAddress { get; set; }
        public string RoomType { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public string UserImage { get; set; }
        public string UtilityNames { get; set; }
        public string UtilityDescriptions { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
