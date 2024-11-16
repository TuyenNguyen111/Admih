namespace FinalProject.Data;

public partial class RoomUtility
{
    public int RoomUtilityId { get; set; }

    public int? PostId { get; set; }

    public int? UtilityId { get; set; }

    public virtual RoomPost? Post { get; set; }

    public virtual Utility? Utility { get; set; }
}
