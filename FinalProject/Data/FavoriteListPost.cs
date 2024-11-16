namespace FinalProject.Data;

public partial class FavoriteListPost
{
    public int FavoriteListPostId { get; set; }

    public int? PostId { get; set; }

    public int? FavoriteId { get; set; }

    public virtual FavoriteList? Favorite { get; set; }

    public virtual RoomPost? Post { get; set; }
}
