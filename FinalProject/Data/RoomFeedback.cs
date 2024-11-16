namespace FinalProject.Data;

public partial class RoomFeedback
{
    public int RoomFeedbackId { get; set; }

    public int? UserId { get; set; }

    public int? PostId { get; set; }

    public int? FeedbackId { get; set; }

    public DateTime? Date { get; set; }

    public virtual User? User { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual RoomPost? Post { get; set; }
    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
}
