using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Itsm.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الطلب مطلوب")]
        [Display(Name = "عنوان الطلب")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "وصف المشكلة مطلوب")]
        [Display(Name = "وصف المشكلة")]
        public string Description { get; set; } = string.Empty;

        // --- بيانات مقدم الطلب ---
        [Required(ErrorMessage = "اسم الموظف مطلوب")]
        [Display(Name = "اسم الموظف")]
        [ValidateNever]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الجهة مطلوبة")]
        [Display(Name = "الجهة")]
        [ValidateNever]
        public string Organization { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        [Display(Name = "رقم الجوال")]
        [ValidateNever]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح")]
        [ValidateNever]
        public string EmployeeEmail { get; set; } = string.Empty;
        // -------------------------

        // مستوى التأثير: يحدده مدير الاستقبال عند قبول التذكرة (قبلها: غير محدد)
        [Display(Name = "مستوى التأثير")]
        public RiskLevel Risk { get; set; } = RiskLevel.Unset;

        [Display(Name = "حالة التذكرة")]
        public TicketStatus Status { get; set; } = TicketStatus.PendingApproval;

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "وقت بدء الـ SLA")]
        public DateTime? SlaStartTime { get; set; }

        [Display(Name = "الموعد النهائي للـ SLA")]
        public DateTime? SlaDeadline { get; set; }

        // موعد الاستجابة المطلوب (من اتفاقية المشروع) ويُحسب عند القبول
        public DateTime? ResponseDeadline { get; set; }

        [ValidateNever] public string EmployeeId { get; set; } = string.Empty;
        [ValidateNever] public string AssignedEngineerId { get; set; } = string.Empty;

        // المشروع الذي يخصه الطلب
        public int? ProjectId { get; set; }
        [ValidateNever] public Project? Project { get; set; }

        // قرار الموافقة / الرفض
        [ValidateNever] public string DecidedBy { get; set; } = string.Empty;
        public DateTime? DecidedAt { get; set; }
        [ValidateNever] public string RejectionReason { get; set; } = string.Empty;

        // الحل والإغلاق
        [ValidateNever] public string Solution { get; set; } = string.Empty;
        public DateTime? ClosedAt { get; set; }

        // إيقاف عداد SLA وتقرير الفني
        public DateTime? SlaStoppedAt { get; set; }
        [ValidateNever] public string TechnicianNotes { get; set; } = string.Empty;

        // تقييم الموظف للفني (1 إلى 5) بعد الإغلاق
        public int? Rating { get; set; }
        [ValidateNever] public string RatingComment { get; set; } = string.Empty;
        public DateTime? RatedAt { get; set; }
        // بيانات المقيِّم وردّ مدير النظام
        [ValidateNever] public string ReviewerName { get; set; } = string.Empty;
        [ValidateNever] public string ReviewerContact { get; set; } = string.Empty;
        [ValidateNever] public string AdminReply { get; set; } = string.Empty;
        public DateTime? AdminReplyAt { get; set; }

        // علامات تنبيهات SLA (تمنع تكرار التنبيه)
        public bool SlaWarningSent { get; set; }
        public bool SlaBreachSent { get; set; }

        [ValidateNever] public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    }

    public enum RiskLevel
    {
        [Display(Name = "لم يُحدَّد")] Unset = 0,
        [Display(Name = "منخفضة")] Low = 1,
        [Display(Name = "متوسطة")] Medium = 2,
        [Display(Name = "عالية")] High = 3,
        [Display(Name = "حرجة")] Critical = 4
    }

    public enum TicketStatus
    {
        [Display(Name = "بانتظار الموافقة")] PendingApproval = 0,
        [Display(Name = "موجّهة للفني")] Approved = 1,
        [Display(Name = "مرفوضة")] Rejected = 2,
        [Display(Name = "قيد المعالجة")] InProgress = 3,
        [Display(Name = "مغلقة")] Closed = 4,
        [Display(Name = "تم الحل بانتظار الاعتماد")] PendingValidation = 5
    }
}