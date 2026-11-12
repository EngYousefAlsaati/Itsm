namespace Itsm.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public string UploaderRole { get; set; } = string.Empty;
    }
}