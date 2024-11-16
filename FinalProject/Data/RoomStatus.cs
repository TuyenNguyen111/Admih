namespace FinalProject.Data;

public partial class RoomStatus
{
    public int RoomStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<RoomPost> RoomPosts { get; set; } = new List<RoomPost>();
}
