namespace FinalProject.Data
{
    public class Response
    {
        public int ResponseId { get; set; }
        public int RoomFeedbackId { get; set; } 
        public string ResponseText { get; set; }
        public DateTime ResponseDate { get; set; }
        public virtual RoomFeedback RoomFeedback { get; set; }
    }
}
