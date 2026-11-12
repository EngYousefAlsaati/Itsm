using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Itsm.Controllers
{
    // تقييم دوري عام للفنيين: يقيّم الموظف ومدير الاستقبال (10 بنود) مرة في كل دورة،
    // متى أتاح مدير النظام ذلك
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? Email => HttpContext.Session.GetString("UserEmail");
        private string? Role => HttpContext.Session.GetString("UserRole");

        // هل التقييم متاح لهذه الفئة؟ (Employee أو Vendor)
        private async Task<bool> EnabledAsync(string role)
        {
            var key = role == "Employee" ? SettingKeys.RatingEmployees : SettingKeys.RatingManagers;
            return await _context.AppSettings.AnyAsync(s => s.Key == key && s.Value == "true");
        }

        private async Task<int> PeriodMonthsAsync()
        {
            var s = await _context.AppSettings.FindAsync(SettingKeys.RatingPeriodMonths);
            return RatingPeriod.NormalizeMonths(s?.Value);
        }

        // الفنيون الذين يحق للمستخدم الحالي تقييمهم في الدورة الحالية (ولم يقيّمهم بعد) مع عدد التذاكر التي خدموا فيها
        private async Task<List<KeyValuePair<string, int>>> PendingAsync(string periodKey)
        {
            var role = Role;
            var email = Email;
            var result = new List<KeyValuePair<string, int>>();
            if (string.IsNullOrEmpty(email) || (role != "Employee" && role != "Vendor")) return result;

            var (start, end) = RatingPeriod.Range(periodKey);

            var evaluated = await _context.TechnicianEvaluations
                .Where(e => e.EvaluatorEmail == email && e.PeriodKey == periodKey)
                .Select(e => e.TechnicianEmail)
                .ToListAsync();

            if (role == "Vendor")
            {
                var techs = await _context.Users
                    .Where(u => u.Role == "Technician" && u.IsActive)
                    .Select(u => u.Email)
                    .ToListAsync();

                var counts = await _context.Tickets
                    .Where(t => t.Status == TicketStatus.Closed && t.AssignedEngineerId != "" && t.ClosedAt >= start && t.ClosedAt < end)
                    .GroupBy(t => t.AssignedEngineerId)
                    .Select(g => new { g.Key, N = g.Count() })
                    .ToListAsync();

                foreach (var t in techs.Where(t => !evaluated.Contains(t, StringComparer.OrdinalIgnoreCase)))
                {
                    var n = counts.FirstOrDefault(c => string.Equals(c.Key, t, StringComparison.OrdinalIgnoreCase))?.N ?? 0;
                    result.Add(new KeyValuePair<string, int>(t, n));
                }
            }
            else
            {
                var served = await _context.Tickets
                    .Where(t => t.EmployeeId == email && t.Status == TicketStatus.Closed && t.AssignedEngineerId != ""
                                && t.ClosedAt >= start && t.ClosedAt < end)
                    .GroupBy(t => t.AssignedEngineerId)
                    .Select(g => new { g.Key, N = g.Count() })
                    .ToListAsync();

                foreach (var s in served.Where(s => !evaluated.Contains(s.Key, StringComparer.OrdinalIgnoreCase)))
                {
                    result.Add(new KeyValuePair<string, int>(s.Key, s.N));
                }
            }

            return result.OrderBy(r => r.Key).ToList();
        }

        // صفحة التقييمات: يراها جميع المستخدمين
        public async Task<IActionResult> Index(string? period)
        {
            var email = Email;
            var role = Role;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var months = await PeriodMonthsAsync();
            var currentKey = RatingPeriod.Key(months, DateTime.Now);

            var existingKeys = await _context.TechnicianEvaluations
                .Select(e => e.PeriodKey)
                .Distinct()
                .ToListAsync();
            if (!existingKeys.Contains(currentKey)) existingKeys.Add(currentKey);

            var selected = string.IsNullOrWhiteSpace(period) ? currentKey : period;
            var showAll = selected == "all";

            IQueryable<TechnicianEvaluation> query = _context.TechnicianEvaluations;
            if (!showAll)
            {
                query = query.Where(e => e.PeriodKey == selected);
            }
            var evaluations = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();

            var techs = await _context.Users
                .Where(u => u.Role == "Technician" && u.IsActive)
                .Select(u => u.Email)
                .ToListAsync();
            foreach (var t in evaluations.Select(e => e.TechnicianEmail))
            {
                if (!techs.Contains(t, StringComparer.OrdinalIgnoreCase)) techs.Add(t);
            }

            var employeesEnabled = await EnabledAsync("Employee");
            var managersEnabled = await EnabledAsync("Vendor");

            ViewBag.Technicians = techs.OrderBy(x => x).ToList();
            ViewBag.EmployeesEnabled = employeesEnabled;
            ViewBag.ManagersEnabled = managersEnabled;
            ViewBag.Months = months;
            ViewBag.CurrentKey = currentKey;
            ViewBag.SelectedKey = selected;
            ViewBag.Periods = existingKeys.OrderByDescending(k => k).ToList();

            var enabledForMe = (role == "Employee" && employeesEnabled) || (role == "Vendor" && managersEnabled);
            ViewBag.Pending = enabledForMe ? await PendingAsync(currentKey) : new List<KeyValuePair<string, int>>();

            return View(evaluations);
        }

        // نص الخطأ إن كان المستخدم لا يستطيع تقييم هذا الفني الآن، أو null إن كان مسموحاً
        private async Task<string?> CanEvaluateAsync(string technician, string periodKey)
        {
            var role = Role;
            var email = Email;

            if (role != "Employee" && role != "Vendor")
                return "التقييم متاح للموظف ومدير الاستقبال فقط.";
            if (!await EnabledAsync(role))
                return "التقييم غير مفعّل لفئتك حالياً. يتيحه مدير النظام.";
            if (!await _context.Users.AnyAsync(u => u.Email == technician && u.Role == "Technician" && u.IsActive))
                return "الفني المختار غير صالح.";
            if (await _context.TechnicianEvaluations.AnyAsync(e => e.TechnicianEmail == technician && e.EvaluatorEmail == email && e.PeriodKey == periodKey))
                return "قيّمت هذا الفني مسبقاً في هذه الدورة.";

            if (role == "Employee")
            {
                var (start, end) = RatingPeriod.Range(periodKey);
                var served = await _context.Tickets.AnyAsync(t => t.EmployeeId == email && t.Status == TicketStatus.Closed
                    && t.AssignedEngineerId == technician && t.ClosedAt >= start && t.ClosedAt < end);
                if (!served)
                    return "يمكنك تقييم الفنيين الذين أغلقوا لك تذاكر خلال هذه الدورة فقط.";
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Rate(string technician)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("Login", "Account");
            }

            technician = (technician ?? string.Empty).Trim().ToLowerInvariant();
            var months = await PeriodMonthsAsync();
            var key = RatingPeriod.Key(months, DateTime.Now);

            var error = await CanEvaluateAsync(technician, key);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            ViewBag.EvaluatorName = me?.FullName ?? string.Empty;
            ViewBag.PeriodLabel = RatingPeriod.Label(key);
            ViewBag.Technician = technician;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(string technician, int[] scores, string? comment, string? reviewerName, string? reviewerContact)
        {
            var email = Email;
            var role = Role;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            technician = (technician ?? string.Empty).Trim().ToLowerInvariant();
            var months = await PeriodMonthsAsync();
            var key = RatingPeriod.Key(months, DateTime.Now);

            var error = await CanEvaluateAsync(technician, key);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            if (scores == null || scores.Length != RatingCriteria.Items.Length || scores.Any(s => s < 1 || s > 5))
            {
                TempData["Error"] = "قيّم جميع البنود العشرة بدرجة من 1 إلى 5.";
                return RedirectToAction(nameof(Rate), new { technician });
            }

            var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            string name;
            string contact;
            if (role == "Employee")
            {
                name = (reviewerName ?? string.Empty).Trim();
                contact = (reviewerContact ?? string.Empty).Trim();
                if (name.Length == 0 || contact.Length == 0)
                {
                    TempData["Error"] = "اكتب اسمك ووسيلة التواصل قبل إرسال التقييم.";
                    return RedirectToAction(nameof(Rate), new { technician });
                }
            }
            else
            {
                name = string.IsNullOrWhiteSpace(me?.FullName) ? email : me!.FullName;
                contact = email;
            }

            var average = Math.Round(scores.Average(), 2);
            var evaluatorLabel = role == "Employee" ? "الموظف" : "مدير الاستقبال";
            var cleanComment = (comment ?? string.Empty).Trim();
            if (cleanComment.Length > 2000) cleanComment = cleanComment.Substring(0, 2000);

            _context.TechnicianEvaluations.Add(new TechnicianEvaluation
            {
                TechnicianEmail = technician,
                EvaluatorRole = role!,
                EvaluatorEmail = email,
                EvaluatorName = name.Length > 200 ? name.Substring(0, 200) : name,
                EvaluatorContact = contact.Length > 200 ? contact.Substring(0, 200) : contact,
                PeriodKey = key,
                ScoresCsv = string.Join(",", scores),
                Average = average,
                Comment = cleanComment,
                CreatedAt = DateTime.Now
            });

            _context.Notifications.Add(new Notification
            {
                UserEmail = technician,
                Message = $"قيّمك {evaluatorLabel} ضمن تقييم {RatingPeriod.Label(key)}: {average:0.0} من 5",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "شكراً، تم حفظ تقييمك ويراه جميع المستخدمين.";
            return RedirectToAction(nameof(Index));
        }

        // رد مدير النظام على تقييم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string reply)
        {
            if (Role != "Admin")
            {
                return RedirectToAction(nameof(Index));
            }

            var evaluation = await _context.TechnicianEvaluations.FindAsync(id);
            if (evaluation == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(reply))
            {
                TempData["Error"] = "اكتب نص الرد أولاً.";
                return RedirectToAction(nameof(Index));
            }

            reply = reply.Trim();
            evaluation.AdminReply = reply.Length > 2000 ? reply.Substring(0, 2000) : reply;
            evaluation.AdminReplyAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حفظ الرد.";
            return RedirectToAction(nameof(Index), new { period = "all" });
        }

        // حذف تقييم (مدير النظام): يستطيع المقيِّم بعده إعادة التقييم في الدورة نفسها
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (Role != "Admin")
            {
                return RedirectToAction(nameof(Index));
            }

            var evaluation = await _context.TechnicianEvaluations.FindAsync(id);
            if (evaluation == null)
            {
                return NotFound();
            }

            _context.TechnicianEvaluations.Remove(evaluation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف التقييم.";
            return RedirectToAction(nameof(Index), new { period = "all" });
        }

        // مدير النظام: إتاحة التقييم للموظفين ولمدراء الاستقبال وتحديد دورة التقييم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSettings(string? employees, string? managers, string? periodMonths)
        {
            if (Role != "Admin")
            {
                return RedirectToAction(nameof(Index));
            }

            async Task SetAsync(string key, string value)
            {
                var s = await _context.AppSettings.FindAsync(key);
                if (s == null)
                {
                    _context.AppSettings.Add(new AppSetting { Key = key, Value = value });
                }
                else
                {
                    s.Value = value;
                }
            }

            await SetAsync(SettingKeys.RatingEmployees, employees == "true" ? "true" : "false");
            await SetAsync(SettingKeys.RatingManagers, managers == "true" ? "true" : "false");
            await SetAsync(SettingKeys.RatingPeriodMonths, RatingPeriod.NormalizeMonths(periodMonths).ToString());
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حفظ إعدادات التقييم.";
            return RedirectToAction(nameof(Index));
        }
    }
}