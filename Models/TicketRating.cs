using System.ComponentModel.DataAnnotations;

namespace Itsm.Models
{
    // تقييم دوري عام لفني: بنود عشرة من 1 إلى 5، من موظف أو من مدير الاستقبال، مرة لكل دورة
    public class TechnicianEvaluation
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string TechnicianEmail { get; set; } = string.Empty;

        // Employee | Vendor (مدير الاستقبال)
        [StringLength(20)]
        public string EvaluatorRole { get; set; } = string.Empty;

        [StringLength(200)]
        public string EvaluatorEmail { get; set; } = string.Empty;

        [StringLength(200)]
        public string EvaluatorName { get; set; } = string.Empty;

        // وسيلة تواصل المقيِّم: تظهر لمدير النظام فقط
        [StringLength(200)]
        public string EvaluatorContact { get; set; } = string.Empty;

        // مفتاح الدورة: 2026-H1 أو 2026-H2 (كل 6 أشهر)، أو 2026 (سنوي)
        [StringLength(20)]
        public string PeriodKey { get; set; } = string.Empty;

        // الدرجات العشر مفصولة بفواصل، مثل: 5,4,5,3,5,5,4,4,5,5
        [StringLength(100)]
        public string ScoresCsv { get; set; } = string.Empty;

        // متوسط الدرجات العشر
        public double Average { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // رد مدير النظام على التقييم
        [StringLength(2000)]
        public string AdminReply { get; set; } = string.Empty;
        public DateTime? AdminReplyAt { get; set; }

        public int[] GetScores()
        {
            return (ScoresCsv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out var v) ? v : 0)
                .ToArray();
        }
    }

    // إعدادات عامة للنظام (مفتاح / قيمة)
    public class AppSetting
    {
        [Key, StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [StringLength(500)]
        public string Value { get; set; } = string.Empty;
    }

    public static class SettingKeys
    {
        // إتاحة التقييم للموظفين / لمدراء الاستقبال (القيمة "true" أو "false")
        public const string RatingEmployees = "Rating.Employees";
        public const string RatingManagers = "Rating.Managers";

        // دورة التقييم بالأشهر: 6 أو 12
        public const string RatingPeriodMonths = "Rating.PeriodMonths";
    }

    // بنود التقييم العشرة (عدّلها من هنا فقط)
    public static class RatingCriteria
    {
        public static readonly (string Ar, string En)[] Items =
        {
            ("سرعة الاستجابة للتذاكر", "Response speed"),
            ("جودة الحلول المقدّمة", "Quality of solutions"),
            ("الالتزام بمهل SLA", "SLA compliance"),
            ("المهارة والكفاءة التقنية", "Technical skill and competence"),
            ("التواصل والتنسيق", "Communication and coordination"),
            ("الاحترافية وحسن التعامل", "Professionalism and courtesy"),
            ("وضوح تقارير الحل وتوثيقها", "Clarity of solution reports"),
            ("الالتزام بالإجراءات وسياسات العمل", "Compliance with procedures"),
            ("التعاون مع الفريق", "Teamwork and cooperation"),
            ("الرضا العام عن الخدمة", "Overall satisfaction")
        };
    }

    // دورات التقييم: نصف سنوية (H1: يناير–يونيو، H2: يوليو–ديسمبر) أو سنوية
    public static class RatingPeriod
    {
        public static int NormalizeMonths(string? value) => value == "12" ? 12 : 6;

        public static string Key(int months, DateTime date) =>
            months == 12 ? date.Year.ToString() : $"{date.Year}-H{(date.Month <= 6 ? 1 : 2)}";

        // نطاق الدورة [البداية، النهاية الحصرية)
        public static (DateTime Start, DateTime End) Range(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length < 4 || !int.TryParse(key.Substring(0, 4), out var year))
            {
                return (DateTime.MinValue, DateTime.MinValue);
            }

            if (key.Contains("-H"))
            {
                var start = key.EndsWith("H2") ? new DateTime(year, 7, 1) : new DateTime(year, 1, 1);
                return (start, start.AddMonths(6));
            }

            var yearStart = new DateTime(year, 1, 1);
            return (yearStart, yearStart.AddYears(1));
        }

        public static string Label(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length < 4) return key;
            var year = key.Substring(0, 4);

            if (key.EndsWith("H1")) return $"النصف الأول {year} (يناير – يونيو)";
            if (key.EndsWith("H2")) return $"النصف الثاني {year} (يوليو – ديسمبر)";
            return $"عام {year}";
        }
    }
}