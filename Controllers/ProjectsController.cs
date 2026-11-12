using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Itsm.Controllers
{
    // إدارة المشاريع: لمدير النظام فقط
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };

        public ProjectsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";

        private IActionResult? Guard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (!IsAdmin)
            {
                TempData["Error"] = "هذه الصفحة لمدير النظام فقط.";
                return RedirectToAction("Dashboard", "Home");
            }
            return null;
        }

        // يتحقق من مدخلات المشروع ويرجع نص الخطأ أو null
        private static string? Validate(string? number, string? name, DateTime startDate, int value, string? unit)
        {
            if (string.IsNullOrWhiteSpace(number)) return "رقم المشروع مطلوب.";
            if (number.Trim().Length > 50) return "رقم المشروع طويل جداً.";
            if (string.IsNullOrWhiteSpace(name)) return "اسم المشروع مطلوب.";
            if (name.Trim().Length > 300) return "اسم المشروع طويل جداً.";
            if (unit != "Days" && unit != "Months") return "اختر وحدة المدة (أيام أو أشهر).";
            if (startDate.Year < 2000 || startDate.Year > 2100) return "تاريخ البداية غير صالح.";
            if (value < 1) return "مدة المشروع يجب أن تكون 1 على الأقل.";
            if (unit == "Days" && value > 3650) return "الحد الأقصى للمدة 3650 يوماً.";
            if (unit == "Months" && value > 120) return "الحد الأقصى للمدة 120 شهراً.";
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var guard = Guard();
            if (guard != null) return guard;

            var projects = await _context.Projects
                .Include(p => p.Organization)
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.EndDate)
                .ToListAsync();

            ViewBag.Orgs = await _context.Organizations.Where(o => o.IsActive).OrderBy(o => o.Name).ToListAsync();
            return View(projects);
        }

        // حساب تاريخ النهاية للمعاينة الفورية في النموذج
        [HttpGet]
        public IActionResult CalcEnd(DateTime start, int value, string unit)
        {
            if (!IsAdmin || Validate("x", "x", start, value, unit) != null)
            {
                return Json(new { end = "" });
            }
            return Json(new { end = Project.CalcEnd(start, value, unit).ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string projectNumber, string name, int organizationId,
            DateTime startDate, int durationValue, string durationUnit)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var error = Validate(projectNumber, name, startDate, durationValue, durationUnit);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            projectNumber = projectNumber.Trim();

            if (!await _context.Organizations.AnyAsync(o => o.Id == organizationId && o.IsActive))
            {
                TempData["Error"] = "اختر عميلاً صالحاً.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Projects.AnyAsync(p => p.ProjectNumber == projectNumber))
            {
                TempData["Error"] = "رقم المشروع مسجّل مسبقاً.";
                return RedirectToAction(nameof(Index));
            }

            var newProject = new Project
            {
                ProjectNumber = projectNumber,
                Name = name.Trim(),
                OrganizationId = organizationId,
                StartDate = startDate.Date,
                DurationValue = durationValue,
                DurationUnit = durationUnit,
                EndDate = Project.CalcEnd(startDate, durationValue, durationUnit),
                IsActive = true
            };
            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            // اتفاقية مستوى الخدمة الافتراضية للمشروع (يعدّلها مدير النظام لاحقاً)
            _context.ProjectSlas.AddRange(SlaDefaults.Create(newProject.Id));
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تمت إضافة المشروع «{name.Trim()}».";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Orgs = await _context.Organizations.OrderBy(o => o.Name).ToListAsync();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string projectNumber, string name, int organizationId,
            DateTime startDate, int durationValue, string durationUnit)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var error = Validate(projectNumber, name, startDate, durationValue, durationUnit);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Edit), new { id });
            }

            projectNumber = projectNumber.Trim();

            if (!await _context.Organizations.AnyAsync(o => o.Id == organizationId))
            {
                TempData["Error"] = "اختر عميلاً صالحاً.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            if (await _context.Projects.AnyAsync(p => p.ProjectNumber == projectNumber && p.Id != id))
            {
                TempData["Error"] = "رقم المشروع مسجّل لمشروع آخر.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            project.ProjectNumber = projectNumber;
            project.Name = name.Trim();
            project.OrganizationId = organizationId;
            project.StartDate = startDate.Date;
            project.DurationValue = durationValue;
            project.DurationUnit = durationUnit;
            project.EndDate = Project.CalcEnd(startDate, durationValue, durationUnit);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حفظ بيانات المشروع.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id, bool active)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.IsActive = active;
            await _context.SaveChangesAsync();

            TempData["Success"] = active ? $"تم تفعيل المشروع «{project.Name}»." : $"تم إيقاف المشروع «{project.Name}».";
            return RedirectToAction(nameof(Index));
        }

        // ---------- اتفاقيات مستوى الخدمة ----------

        // ينشئ صفوف الاتفاقية الناقصة (للمشاريع الأقدم) من القالب الافتراضي
        private async Task EnsureSlaAsync(int projectId)
        {
            var existing = await _context.ProjectSlas
                .Where(s => s.ProjectId == projectId)
                .Select(s => s.Level)
                .ToListAsync();

            var missing = SlaDefaults.Create(projectId).Where(s => !existing.Contains(s.Level)).ToList();
            if (missing.Count > 0)
            {
                _context.ProjectSlas.AddRange(missing);
                await _context.SaveChangesAsync();
            }

            // ترقية الاتفاقيات القديمة: النص الإنجليزي القديم ينتقل لحقل الإنجليزية ويُوضع النص العربي الافتراضي
            var rows = await _context.ProjectSlas.Where(s => s.ProjectId == projectId).ToListAsync();
            var defaults = SlaDefaults.Create(projectId).ToDictionary(d => d.Level);
            var changed = false;

            foreach (var row in rows)
            {
                var hasArabic = row.Description.Any(c => c >= '\u0600' && c <= '\u06FF');
                if (string.IsNullOrWhiteSpace(row.DescriptionEn) && !string.IsNullOrWhiteSpace(row.Description) && !hasArabic)
                {
                    row.DescriptionEn = row.Description;
                    if (defaults.TryGetValue(row.Level, out var d))
                    {
                        row.Description = d.Description;
                    }
                    changed = true;
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }
        }

        // من يحق له الاطلاع على اتفاقية المشروع
        private async Task<bool> CanViewAsync(Project project)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email)) return false;

            if (role == "Admin" || role == "Vendor" || role == "Technician") return true;

            if (role == "Employee")
            {
                var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return me?.OrganizationId == project.OrganizationId;
            }
            return false;
        }

        // قائمة المشاريع التي يرى المستخدم اتفاقياتها
        public async Task<IActionResult> Agreements()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<Project> query = _context.Projects.Include(p => p.Organization);

            if (role == "Employee")
            {
                var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                var orgId = me?.OrganizationId;
                query = query.Where(p => orgId != null && p.OrganizationId == orgId);
            }

            var projects = await query
                .OrderByDescending(p => p.IsActive)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return View(projects);
        }

        // عرض اتفاقية مستوى الخدمة لمشروع
        public async Task<IActionResult> Sla(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            var project = await _context.Projects.Include(p => p.Organization).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (!await CanViewAsync(project))
            {
                TempData["Error"] = "ليست لديك صلاحية للاطلاع على هذه الاتفاقية.";
                return RedirectToAction("Dashboard", "Home");
            }

            await EnsureSlaAsync(id);

            ViewBag.Levels = await _context.ProjectSlas
                .Where(s => s.ProjectId == id)
                .OrderByDescending(s => s.Level)
                .ToListAsync();
            ViewBag.CanEdit = IsAdmin;

            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> SlaEdit(int id)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var project = await _context.Projects.Include(p => p.Organization).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            await EnsureSlaAsync(id);

            ViewBag.Levels = await _context.ProjectSlas
                .Where(s => s.ProjectId == id)
                .OrderByDescending(s => s.Level)
                .ToListAsync();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSla(int id, List<SlaRowInput> rows)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            await EnsureSlaAsync(id);
            var levels = await _context.ProjectSlas.Where(s => s.ProjectId == id).ToListAsync();

            foreach (var r in rows ?? new List<SlaRowInput>())
            {
                if (r.Level < 1 || r.Level > 4) continue;

                var row = levels.FirstOrDefault(s => (int)s.Level == r.Level);
                if (row == null) continue;

                if (r.ResponseMinutes < 1 || r.ResponseMinutes > 100000 ||
                    r.ResolutionMinutes < 1 || r.ResolutionMinutes > 1000000)
                {
                    TempData["Error"] = "أزمنة الاستجابة والحل يجب أن تكون بالدقائق وأكبر من صفر.";
                    return RedirectToAction(nameof(SlaEdit), new { id });
                }

                var desc = (r.Description ?? string.Empty).Replace("\r", string.Empty).Trim();
                if (desc.Length > 2000)
                {
                    TempData["Error"] = "وصف المستوى طويل جداً (الحد 2000 حرف).";
                    return RedirectToAction(nameof(SlaEdit), new { id });
                }

                var descEn = (r.DescriptionEn ?? string.Empty).Replace("\r", string.Empty).Trim();
                if (descEn.Length > 2000)
                {
                    TempData["Error"] = "الوصف الإنجليزي طويل جداً (الحد 2000 حرف).";
                    return RedirectToAction(nameof(SlaEdit), new { id });
                }

                row.Description = desc;
                row.DescriptionEn = descEn;
                row.ResponseMinutes = r.ResponseMinutes;
                row.ResolutionMinutes = r.ResolutionMinutes;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حفظ اتفاقية مستوى الخدمة.";
            return RedirectToAction(nameof(Sla), new { id });
        }

        // ترجمة نص الاتفاقية بين العربية والإنجليزية (لمدير النظام، وتحتاج مفتاح خدمة الترجمة في الإعدادات)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest req)
        {
            if (!IsAdmin)
            {
                return Json(new { ok = false, message = "غير مصرّح." });
            }

            var text = (req?.Text ?? string.Empty).Trim();
            if (text.Length == 0)
            {
                return Json(new { ok = false, message = "لا يوجد نص للترجمة." });
            }
            if (text.Length > 3000)
            {
                return Json(new { ok = false, message = "النص طويل جداً للترجمة (الحد 3000 حرف)." });
            }

            var apiKey = _config["Anthropic:ApiKey"] ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Json(new { ok = false, message = "الترجمة التلقائية غير مفعّلة بعد (لم يُضبط مفتاح الخدمة في الإعدادات). اكتب النص باللغتين يدوياً." });
            }

            var toEnglish = req?.To == "en";
            var system = toEnglish
                ? "You are a professional translator for IT service management (SLA) documents. Translate the user's Arabic text into clear, formal English. Keep line breaks exactly as in the input (each line is a separate bullet). Output only the translation, with no explanations."
                : "You are a professional translator for IT service management (SLA) documents. Translate the user's English text into clear, formal Modern Standard Arabic. Keep line breaks exactly as in the input (each line is a separate bullet). Output only the translation, with no explanations.";

            var payload = new
            {
                model = "claude-haiku-4-5-20251001",
                max_tokens = 2000,
                system = system,
                messages = new[] { new { role = "user", content = text } }
            };

            using var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            msg.Headers.Add("x-api-key", apiKey);
            msg.Headers.Add("anthropic-version", "2023-06-01");
            msg.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                using var resp = await Http.SendAsync(msg);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return Json(new { ok = false, message = $"تعذّرت الترجمة (رمز الخطأ {(int)resp.StatusCode}). تحقق من مفتاح الخدمة." });
                }

                using var doc = JsonDocument.Parse(body);
                var translated = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
                return Json(new { ok = true, text = (translated ?? string.Empty).Trim() });
            }
            catch (Exception)
            {
                return Json(new { ok = false, message = "تعذّر الاتصال بخدمة الترجمة." });
            }
        }
    }

    // طلب الترجمة من نموذج التعديل
    public class TranslateRequest
    {
        public string? Text { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
    }

    // مدخلات صف الاتفاقية من نموذج التعديل
    public class SlaRowInput
    {
        public int Level { get; set; }
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        public int ResponseMinutes { get; set; }
        public int ResolutionMinutes { get; set; }
    }
}