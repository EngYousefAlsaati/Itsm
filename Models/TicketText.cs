namespace Itsm.Models
{
    // نصوص عربية مشتركة للتصدير والطباعة والواجهات
    public static class TicketText
    {
        public static string Status(TicketStatus s) => s switch
        {
            TicketStatus.PendingApproval => "بانتظار الموافقة",
            TicketStatus.Approved => "موجّهة للفني",
            TicketStatus.Rejected => "مرفوضة",
            TicketStatus.InProgress => "قيد المعالجة",
            TicketStatus.Closed => "مغلقة",
            TicketStatus.PendingValidation => "تم الحل بانتظار الاعتماد",
            _ => s.ToString()
        };

        // الموظف لا يرى «بانتظار الاعتماد»: تبقى التذكرة عنده «قيد المعالجة» حتى يصادق مدير الاستقبال
        public static string StatusFor(TicketStatus s, string? role) =>
            role == "Employee" && s == TicketStatus.PendingValidation
                ? Status(TicketStatus.InProgress)
                : Status(s);

        public static string StatusCss(TicketStatus s, string? role = null)
        {
            if (role == "Employee" && s == TicketStatus.PendingValidation) s = TicketStatus.InProgress;
            return s switch
            {
                TicketStatus.PendingApproval => "bg-secondary",
                TicketStatus.Approved => "bg-primary",
                TicketStatus.Rejected => "bg-danger",
                TicketStatus.InProgress => "bg-info text-dark",
                TicketStatus.Closed => "bg-success",
                TicketStatus.PendingValidation => "bg-warning text-dark",
                _ => "bg-secondary"
            };
        }

        public static string Risk(RiskLevel r) => r switch
        {
            RiskLevel.Critical => "حرج",
            RiskLevel.High => "عالٍ",
            RiskLevel.Medium => "متوسط",
            RiskLevel.Low => "منخفض",
            _ => "لم يُحدَّد"
        };

        public static string RiskCss(RiskLevel r) => r switch
        {
            RiskLevel.Critical => "bg-danger",
            RiskLevel.High => "bg-warning text-dark",
            RiskLevel.Medium => "bg-info text-dark",
            RiskLevel.Low => "bg-success",
            _ => "bg-secondary"
        };

        public static string Role(string? r) => r switch
        {
            "Employee" => "موظف",
            "Technician" => "فني / مهندس",
            "Vendor" => "مدير الاستقبال",
            "Admin" => "مدير النظام",
            _ => r ?? ""
        };

        public static string Stars(int? r) =>
            r is >= 1 and <= 5 ? new string('★', r.Value) + new string('☆', 5 - r.Value) : "—";

        // نتيجة SLA كنص
        public static string SlaResult(Ticket t)
        {
            if (t.Status == TicketStatus.Rejected) return "—";

            if (t.Status == TicketStatus.PendingApproval || t.SlaStartTime == null || t.SlaDeadline == null)
                return "لم يبدأ";

            if (t.Status == TicketStatus.Closed || t.Status == TicketStatus.PendingValidation)
            {
                var stopped = t.SlaStoppedAt ?? t.ClosedAt ?? t.SlaDeadline.Value;
                return stopped <= t.SlaDeadline.Value ? "ضمن المهلة" : "متأخرة";
            }

            return DateTime.Now > t.SlaDeadline.Value ? "متأخرة (جارية)" : "ضمن المهلة (جارية)";
        }
    }
}