using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Itsm.Controllers
{
    // نتيجة إضافة مستخدم ضمن الإضافة الجماعية
    public class BulkUserResult
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Created { get; set; }
    }

    // إدارة المستخدمين: لمدير النظام فقط
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static readonly string[] Roles = { "Employee", "Technician", "Vendor", "Admin" };

        private string? CurrentEmail => HttpContext.Session.GetString("UserEmail");

        private IActionResult? Guard()
        {
            if (string.IsNullOrEmpty(CurrentEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "هذه الصفحة لمدير النظام فقط.";
                return RedirectToAction("Dashboard", "Home");
            }
            return null;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // كلمة مرور عشوائية قوية (بدون أحرف متشابهة مثل O و0 و l و1)
        private static string GeneratePassword()
        {
            const string letters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
            const string digits = "23456789";
            const string all = letters + digits;

            while (true)
            {
                var chars = new char[12];
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
                }
                var pw = new string(chars);
                if (PasswordHelper.ValidateStrength(pw) == null) return pw;
            }
        }

        public async Task<IActionResult> Index()
        {
            var guard = Guard();
            if (guard != null) return guard;

            var users = await _context.Users
                .Include(u => u.Organization)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.Email)
                .ToListAsync();

            ViewBag.Orgs = await _context.Organizations.Where(o => o.IsActive).OrderBy(o => o.Name).ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string fullName, string role, string password, int? organizationId)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var cleanEmail = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (!IsValidEmail(cleanEmail))
            {
                TempData["Error"] = "البريد الإلكتروني غير صالح.";
                return RedirectToAction(nameof(Index));
            }

            if (!Roles.Contains(role))
            {
                TempData["Error"] = "اختر دوراً صحيحاً.";
                return RedirectToAction(nameof(Index));
            }

            var strengthError = PasswordHelper.ValidateStrength(password);
            if (strengthError != null)
            {
                TempData["Error"] = strengthError;
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Users.AnyAsync(u => u.Email == cleanEmail))
            {
                TempData["Error"] = "هذا البريد مسجّل مسبقاً.";
                return RedirectToAction(nameof(Index));
            }

            int? orgId = null;
            if (role == "Employee" && organizationId.HasValue)
            {
                if (!await _context.Organizations.AnyAsync(o => o.Id == organizationId.Value && o.IsActive))
                {
                    TempData["Error"] = "الجهة المختارة غير صالحة.";
                    return RedirectToAction(nameof(Index));
                }
                orgId = organizationId;
            }

            _context.Users.Add(new AppUser
            {
                Email = cleanEmail,
                FullName = string.IsNullOrWhiteSpace(fullName) ? cleanEmail.Split('@')[0] : fullName.Trim(),
                Role = role,
                PasswordHash = PasswordHelper.Hash(password),
                OrganizationId = orgId,
                IsActive = true
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تمت إضافة المستخدم {cleanEmail}.";
            return RedirectToAction(nameof(Index));
        }

        // إضافة عدة موظفين دفعة واحدة لجهة معيّنة، مع كلمات مرور عشوائية تُعرض مرة واحدة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreate(int organizationId, string emails)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId && o.IsActive);
            if (org == null)
            {
                TempData["Error"] = "اختر جهة صالحة.";
                return RedirectToAction(nameof(Index));
            }

            var lines = (emails ?? string.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();

            if (lines.Count == 0)
            {
                TempData["Error"] = "اكتب بريداً واحداً على الأقل (بريد في كل سطر).";
                return RedirectToAction(nameof(Index));
            }
            if (lines.Count > 50)
            {
                TempData["Error"] = "الحد الأقصى 50 بريداً في المرة الواحدة.";
                return RedirectToAction(nameof(Index));
            }

            var results = new List<BulkUserResult>();
            var seen = new HashSet<string>();

            foreach (var line in lines)
            {
                // الصيغة: البريد، أو البريد ثم فاصلة ثم الاسم
                var parts = line.Split(new[] { ',', '،', ';', '؛', '\t' }, 2);
                var email = parts[0].Trim().ToLowerInvariant();
                var name = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                var r = new BulkUserResult { Email = email, Name = name };

                if (!IsValidEmail(email))
                {
                    r.Status = "بريد غير صالح";
                }
                else if (!seen.Add(email))
                {
                    r.Status = "مكرر في القائمة";
                }
                else if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    r.Status = "مسجّل مسبقاً";
                }
                else
                {
                    var password = GeneratePassword();
                    _context.Users.Add(new AppUser
                    {
                        Email = email,
                        FullName = name.Length > 0 ? name : email.Split('@')[0],
                        Role = "Employee",
                        PasswordHash = PasswordHelper.Hash(password),
                        OrganizationId = org.Id,
                        IsActive = true
                    });
                    r.Password = password;
                    r.Created = true;
                    r.Status = "تمت الإضافة";
                }

                results.Add(r);
            }

            await _context.SaveChangesAsync();

            ViewBag.OrgName = org.Name;
            Response.Headers["Cache-Control"] = "no-store";
            return View("BulkResult", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetOrganization(int id, int? organizationId)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Role != "Employee")
            {
                TempData["Error"] = "الجهة تخص الموظفين طالبي الخدمة فقط.";
                return RedirectToAction(nameof(Index));
            }

            if (organizationId.HasValue && !await _context.Organizations.AnyAsync(o => o.Id == organizationId.Value && o.IsActive))
            {
                TempData["Error"] = "الجهة المختارة غير صالحة.";
                return RedirectToAction(nameof(Index));
            }

            user.OrganizationId = organizationId;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تم تحديث جهة {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id, bool active)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!active)
            {
                if (string.Equals(user.Email, CurrentEmail, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "لا يمكنك إيقاف حسابك الحالي.";
                    return RedirectToAction(nameof(Index));
                }

                if (user.Role == "Admin")
                {
                    var otherAdmins = await _context.Users.CountAsync(u => u.Role == "Admin" && u.IsActive && u.Id != user.Id);
                    if (otherAdmins == 0)
                    {
                        TempData["Error"] = "يجب أن يبقى مدير نظام واحد نشط على الأقل.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            user.IsActive = active;
            if (active)
            {
                user.FailedAttempts = 0;
                user.LockoutUntil = null;
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = active ? $"تم تفعيل {user.Email}." : $"تم إيقاف {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var strengthError = PasswordHelper.ValidateStrength(newPassword);
            if (strengthError != null)
            {
                TempData["Error"] = strengthError;
                return RedirectToAction(nameof(Index));
            }

            user.PasswordHash = PasswordHelper.Hash(newPassword);
            user.FailedAttempts = 0;
            user.LockoutUntil = null;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تم تغيير كلمة مرور {user.Email}.";
            return RedirectToAction(nameof(Index));
        }
    }
}