namespace FinalProject.Data;

public partial class FavoriteList
{
    public int FavoriteListId { get; set; }
    public int UserId { get; set; }

    public virtual ICollection<FavoriteListPost> FavoriteListPosts { get; set; } = new List<FavoriteListPost>();
}
