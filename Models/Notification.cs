namespace Itsm.Models
{
    // إشعار موجّه لمستخدم معيّن
    public class Notification
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;   // المستلم
        public string Message { get; set; } = string.Empty;
        public int? TicketId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}