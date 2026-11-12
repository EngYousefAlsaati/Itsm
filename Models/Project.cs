using System.ComponentModel.DataAnnotations;

namespace Itsm.Models
{
    // مشروع يتبع عميلاً معيّناً
    public class Project
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string ProjectNumber { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Name { get; set; } = string.Empty;

        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public DateTime StartDate { get; set; }

        // مدة المشروع ووحدتها: Days أو Months
        public int DurationValue { get; set; }

        [StringLength(10)]
        public string DurationUnit { get; set; } = "Months";

        // تاريخ النهاية يُحسب تلقائياً من البداية والمدة
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // النهاية = البداية + المدة - يوم واحد (مشروع 12 شهراً يبدأ 1 يناير ينتهي 31 ديسمبر)
        public static DateTime CalcEnd(DateTime start, int value, string unit)
        {
            start = start.Date;
            return unit == "Days"
                ? start.AddDays(value - 1)
                : start.AddMonths(value).AddDays(-1);
        }
    }
}