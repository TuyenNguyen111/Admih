namespace FinalProject.Data;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<RoomPost> RoomPosts { get; set; } = new List<RoomPost>();
}
