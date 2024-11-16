namespace FinalProject.Data;

public partial class Utility
{
    public int UtilityId { get; set; }

    public string UtilityName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<RoomUtility> RoomUtilities { get; set; } = new List<RoomUtility>();
}
