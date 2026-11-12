using System.ComponentModel.DataAnnotations;

namespace Itsm.Models
{
    // اتفاقية مستوى الخدمة لمشروع: صف لكل مستوى تأثير
    public class ProjectSla
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        // مستوى التأثير (Low=1, Medium=2, High=3, Critical=4)
        public RiskLevel Level { get; set; }

        // وصف المستوى بالعربية: كل سطر بند مستقل
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        // وصف المستوى بالإنجليزية: كل سطر بند مستقل
        [StringLength(2000)]
        public string DescriptionEn { get; set; } = string.Empty;

        // زمن الاستجابة بالدقائق (من إخطار العميل)
        public int ResponseMinutes { get; set; }

        // زمن الحل بالدقائق (من وقت الاستجابة الأولية)
        public int ResolutionMinutes { get; set; }
    }

    // القالب الافتراضي لاتفاقية كل مشروع جديد، ودوال مساعدة للعرض
    public static class SlaDefaults
    {
        public static List<ProjectSla> Create(int projectId) => new()
        {
            new ProjectSla
            {
                ProjectId = projectId,
                Level = RiskLevel.Critical,
                Description = "فشل كامل في النظام تتعطل فيه وظائف النظام ولا يستطيع المستخدم استخدام النظام بأكمله.\nالأثر: جميع المستخدمين.",
                DescriptionEn = "Complete system failure where system functions are disrupted, and the user is unable to use the entire system.\nImpact: All users.",
                ResponseMinutes = 30,
                ResolutionMinutes = 120
            },
            new ProjectSla
            {
                ProjectId = projectId,
                Level = RiskLevel.High,
                Description = "مشكلة تحقق أيًّا من المعايير التالية:\nفقدان كلي أو جوهري للخدمة أو للتطبيقات عند استخدام نظام الإنتاج.\nفقدان فعلي أو متصوَّر للبيانات أو تلف فيها يجعل جزءًا أساسيًا من نظام الإنتاج غير قابل للاستخدام.\nمنع الاستخدام الفعّال لأي خدمة رئيسية في النظام من قِبل عدد كبير من المستخدمين، خاصةً إذا حدث ذلك خلال ساعات العمل.\nتشير إلى اختراق أمني خطير، مثل تعرّض عدد كبير من المستندات أو البيانات للخطر.",
                DescriptionEn = "An issue that satisfies any of the following criteria:\nA complete or substantial loss of service / or applications when using a Production System.\nReal or perceived data loss or data corruption makes an essential part of the Production System unusable.\nPrevents the effective use of any major System service by many users especially if it occurs during a working hour.\nImplies a serious breach of security – e.g., compromise of large number of documents/data.",
                ResponseMinutes = 60,
                ResolutionMinutes = 300
            },
            new ProjectSla
            {
                ProjectId = projectId,
                Level = RiskLevel.Medium,
                Description = "مشكلة تحقق أيًّا من المعايير التالية:\nتسبب أثرًا متوسطًا على الأعمال في نظام الإنتاج.\nفقدان جزئي لوظائف غير حرجة دون تأثير جوهري على قابلية استخدام النظام.\nمشكلة لا تتسبب في توقف فوري للعمل.\nتسبب إزعاجًا لعدد قليل من مستخدمي النظام.\nخدمة مهمة متاحة ولكن أداؤها ضعيف.",
                DescriptionEn = "An issue that satisfies any of the following criteria:\nCauses results in a medium business impact for a Production System.\nPartial non-critical functionality loss and the issue has no significant effects on the usability of the System.\nIssue that is not causing an immediate work stoppage.\nCauses inconvenience to a small number of system users.\nAn important service is available, but performance is poor.",
                ResponseMinutes = 240,
                ResolutionMinutes = 480
            },
            new ProjectSla
            {
                ProjectId = projectId,
                Level = RiskLevel.Low,
                Description = "مشكلة تحقق أيًّا من المعايير التالية:\nتسبب أثرًا محدودًا جدًا على الأعمال في نظام الإنتاج.\nتسبب إزعاجًا لفرد واحد.\nقد تُسند لمشكلة لا تؤثر على الجودة أو الأداء.\nوظائف فرعية في النظام.\nحالات طلب معلومات عامة، مثل الاستخدام والإعدادات.",
                DescriptionEn = "An issue that satisfies any of the following criteria:\nCauses results in a minimal business impact for a Production System.\nCauses an inconvenience to an individual.\nMay be assigned to an issue with no impact to quality or Performance.\nSub-functions of the System.\nCases of general information requests, such as usage and configuration.",
                ResponseMinutes = 480,
                ResolutionMinutes = 1440
            }
        };

        public static string LevelAr(RiskLevel l) => l switch
        {
            RiskLevel.Critical => "حرج",
            RiskLevel.High => "عالي",
            RiskLevel.Medium => "متوسط",
            _ => "منخفض"
        };

        public static string LevelEn(RiskLevel l) => l switch
        {
            RiskLevel.Critical => "Critical",
            RiskLevel.High => "High",
            RiskLevel.Medium => "Medium",
            _ => "Low"
        };

        // نص إنجليزي للمدة: 30 minutes، 2 hours، 1 day...
        public static string DurationEn(int minutes)
        {
            if (minutes <= 0) return "—";

            if (minutes % 1440 == 0)
            {
                var d = minutes / 1440;
                return d == 1 ? "1 day" : $"{d} days";
            }

            if (minutes < 60) return minutes == 1 ? "1 minute" : $"{minutes} minutes";

            var h = minutes / 60;
            var mi = minutes % 60;
            var hs = h == 1 ? "1 hour" : $"{h} hours";
            return mi == 0 ? hs : $"{hs} {mi} minutes";
        }

        // نص عربي للمدة: 30 دقيقة، ساعتان، 5 ساعات، يوم واحد...
        public static string Duration(int minutes)
        {
            if (minutes <= 0) return "—";

            if (minutes % 1440 == 0)
            {
                var d = minutes / 1440;
                return d == 1 ? "يوم واحد" : d == 2 ? "يومان" : d <= 10 ? $"{d} أيام" : $"{d} يوماً";
            }

            if (minutes < 60) return $"{minutes} دقيقة";

            var h = minutes / 60;
            var m = minutes % 60;
            var hs = h == 1 ? "ساعة واحدة" : h == 2 ? "ساعتان" : h <= 10 ? $"{h} ساعات" : $"{h} ساعة";
            return m == 0 ? hs : $"{hs} و{m} دقيقة";
        }
    }
}