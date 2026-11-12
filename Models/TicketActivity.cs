namespace Itsm.Models
{
    // سجل التذكرة: أحداث النظام (Kind = 0) وتعليقات المستخدمين (Kind = 1)
    public class TicketActivity
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int Kind { get; set; }

        // قناة الظهور:
        //  ""          حدث عام يراه الجميع
        //  "Internal"  حدث داخلي لمدير الاستقبال ومدير النظام فقط
        //  "Employee"  تعليق قناة الموظف (الموظف + مدير الاستقبال + مدير النظام)
        //  "Technician" تعليق قناة الفني (الفني + مدير الاستقبال + مدير النظام)
        public string Channel { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string ActorEmail { get; set; } = string.Empty;
        public string ActorRole { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}