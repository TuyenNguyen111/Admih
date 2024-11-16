namespace FinalProject.Data
{
    public partial class RoomCoordinates
    {
        public int RoomCoordinateId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public virtual ICollection<RoomPost> RoomPosts { get; set; } = new List<RoomPost>();
    }
}
