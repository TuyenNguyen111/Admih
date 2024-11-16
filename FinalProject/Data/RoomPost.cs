namespace FinalProject.Data;

public partial class RoomPost
{
    public int PostId { get; set; }

    public string? RoomName { get; set; }

    public int? Quantity { get; set; }

    public decimal? RoomPrice { get; set; }

    public decimal? RoomSize { get; set; }

    public string? Address { get; set; }

    public string? RoomDescription { get; set; }

    public DateTime? DatePosted { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? StatusId { get; set; }

    public int? UserId { get; set; }

    public int? RoomTypeId { get; set; }
    public int? RoomCoordinateId { get; set; }

    public virtual ICollection<FavoriteListPost> FavoriteListPosts { get; set; } = new List<FavoriteListPost>();

    public virtual ICollection<RoomFeedback> Feedbacks { get; set; } = new List<RoomFeedback>();

    public virtual ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();

    public virtual RoomType? RoomType { get; set; }

    public virtual ICollection<RoomUtility> RoomUtilities { get; set; } = new List<RoomUtility>();

    public virtual RoomStatus? Status { get; set; }
    public virtual RoomCoordinates RoomCoordinate { get; set; }
}
