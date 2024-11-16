namespace FinalProject.Data;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string? Fullname { get; set; }
    public DateTime? Dob {  get; set; }
    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool? Gender { get; set; }

    public string? UserImage { get; set; }

    public string? RandomKey { get; set; }
    public bool IsValid { get; set; }

    public int? UserTypeId { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? TokenCreateAt { get; set; }

    public virtual UserType? UserType { get; set; }
    public virtual ICollection<RoomFeedback> RoomFeedbacks { get; set; } = new List<RoomFeedback>();
}
