namespace FinalProject.Data;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? FeedbackName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<RoomFeedback> RoomFeedbacks { get; set; } = new List<RoomFeedback>();
}
