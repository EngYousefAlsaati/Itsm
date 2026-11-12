using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Itsm.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? CurrentEmail => HttpContext.Session.GetString("UserEmail");

        // صفحة كل الإشعارات
        public async Task<IActionResult> Index()
        {
            var email = CurrentEmail;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var list = await _context.Notifications
                .Where(n => n.UserEmail == email)
                .OrderByDescending(n => n.CreatedAt)
                .Take(100)
                .ToListAsync();

            return View(list);
        }

        // فتح إشعار: يُعلَّم كمقروء ثم ينقلك إلى التذكرة
        public async Task<IActionResult> Open(int id)
        {
            var email = CurrentEmail;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserEmail == email);
            if (n == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!n.IsRead)
            {
                n.IsRead = true;
                await _context.SaveChangesAsync();
            }

            if (n.TicketId.HasValue)
            {
                return RedirectToAction("Details", "Tickets", new { id = n.TicketId.Value });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var email = CurrentEmail;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var unread = await _context.Notifications
                .Where(n => n.UserEmail == email && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // عدد الإشعارات غير المقروءة (يستدعيه الجرس كل 30 ثانية)
        public async Task<IActionResult> Count()
        {
            var email = CurrentEmail;
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { count = 0 });
            }

            var count = await _context.Notifications.CountAsync(n => n.UserEmail == email && !n.IsRead);
            return Json(new { count });
        }
    }
}