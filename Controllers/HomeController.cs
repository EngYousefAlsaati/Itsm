using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Itsm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // لوحة التحكم (الصفحة بعد تسجيل الدخول)
        public async Task<IActionResult> Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // كل مستخدم يحسب إحصائياته على ما يخصه
            IQueryable<Ticket> query = _context.Tickets;
            if (role == "Employee")
            {
                query = query.Where(t => t.EmployeeId == email);
            }
            else if (role == "Technician")
            {
                query = query.Where(t => t.AssignedEngineerId == email);
            }

            // ---------- عدد التذاكر حسب الحالة ----------
            var byStatus = await query
                .GroupBy(t => t.Status)
                .Select(g => new { g.Key, N = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.N);

            int Get(TicketStatus s) => byStatus.TryGetValue(s, out var n) ? n : 0;

            int pending = Get(TicketStatus.PendingApproval);
            int approved = Get(TicketStatus.Approved);
            int pendingValidation = Get(TicketStatus.PendingValidation);
            int inProgress = Get(TicketStatus.InProgress) + pendingValidation;
            int closed = Get(TicketStatus.Closed);
            int rejected = Get(TicketStatus.Rejected);

            // بانتظار قرار مدير الاستقبال: تذاكر تنتظر القبول أو اعتماد الحل
            ViewBag.Pending = pending + pendingValidation;
            ViewBag.Closed = closed;
            ViewBag.Rejected = rejected;
            ViewBag.Open = pending + approved + inProgress;
            // الترتيب: بانتظار الموافقة، موجّهة، قيد المعالجة، مغلقة، مرفوضة
            ViewBag.StatusCounts = new[] { pending, approved, inProgress, closed, rejected };

            // ---------- التذاكر في آخر 6 أشهر ----------
            var now = DateTime.Now;
            var firstMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var dates = await query
                .Where(t => t.CreatedAt >= firstMonth)
                .Select(t => t.CreatedAt)
                .ToListAsync();

            string[] arMonths =
            {
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };

            var monthLabels = new List<string>();
            var monthCounts = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                var m = firstMonth.AddMonths(i);
                monthLabels.Add(arMonths[m.Month - 1] + " " + m.Year);
                monthCounts.Add(dates.Count(d => d.Year == m.Year && d.Month == m.Month));
            }
            ViewBag.MonthLabels = monthLabels.ToArray();
            ViewBag.MonthCounts = monthCounts.ToArray();

            // ---------- الالتزام بـ SLA ومتوسط زمن الحل (للتذاكر المغلقة) ----------
            var closedTimes = await query
                .Where(t => t.Status == TicketStatus.Closed && t.SlaStartTime != null && t.SlaDeadline != null)
                .Select(t => new { t.SlaStartTime, t.SlaDeadline, t.SlaStoppedAt, t.ClosedAt })
                .ToListAsync();

            int onTime = 0, late = 0, counted = 0;
            double totalHours = 0;
            foreach (var c in closedTimes)
            {
                var start = c.SlaStartTime!.Value;
                var deadline = c.SlaDeadline!.Value;
                var stopped = c.SlaStoppedAt ?? c.ClosedAt ?? deadline;

                if (stopped <= deadline) onTime++; else late++;
                totalHours += (stopped - start).TotalHours;
                counted++;
            }

            ViewBag.OnTime = onTime;
            ViewBag.LateClosed = late;
            ViewBag.SlaPct = (onTime + late) == 0 ? 0 : (int)Math.Round(onTime * 100.0 / (onTime + late));
            ViewBag.AvgHours = counted == 0 ? 0.0 : Math.Round(totalHours / counted, 1);

            // تذاكر نشطة تجاوزت موعد SLA
            var active = new[] { TicketStatus.Approved, TicketStatus.InProgress };
            ViewBag.LateActive = await query.CountAsync(t => active.Contains(t.Status) && t.SlaDeadline < now);

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}