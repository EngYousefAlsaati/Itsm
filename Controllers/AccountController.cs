using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Itsm.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // الجهات الابتدائية وشعاراتها (تُنشأ مرة واحدة إذا كان جدول الجهات فارغاً).
        // ملفات الشعارات توضع في wwwroot/images/orgs
        private static readonly (string Name, string Logo)[] SeedOrgs =
        {
            ("مدن", "modon.png"),
            ("وزارة الصحة", "moh.png"),
            ("منشآت", "monshaat.png"),
            ("هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية", "alhayah.png")
        };

        private async Task EnsureOrgSeedAsync()
        {
            if (await _context.Organizations.AnyAsync())
            {
                return;
            }

            var folder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "images", "orgs");

            foreach (var (name, logo) in SeedOrgs)
            {
                _context.Organizations.Add(new Organization
                {
                    Name = name,
                    LogoFileName = System.IO.File.Exists(Path.Combine(folder, logo)) ? logo : string.Empty,
                    IsActive = true
                });
            }
            await _context.SaveChangesAsync();
        }

        // الحسابات الابتدائية: تُنشأ مرة واحدة عند أول تشغيل إذا كان جدول المستخدمين فارغاً.
        // غيّر كلمات المرور هذه بعد أول دخول من صفحة «إدارة المستخدمين».
        private static readonly (string Email, string Password, string Role)[] SeedUsers =
        {
            ("technician1@itsm.com", "Tech@111", "Technician"),
            ("technician2@itsm.com", "Tech@222", "Technician"),
            ("technician3@itsm.com", "Tech@333", "Technician"),
            ("technician4@itsm.com", "Tech@444", "Technician"),

            ("vendor1@itsm.com", "Ven@111", "Vendor"),
            ("vendor2@itsm.com", "Ven@222", "Vendor"),
            ("vendor3@itsm.com", "Ven@333", "Vendor"),
            ("vendor4@itsm.com", "Ven@444", "Vendor"),

            ("admin1@itsm.com", "Admin@111", "Admin"),
            ("admin2@itsm.com", "Admin@222", "Admin"),
            ("admin3@itsm.com", "Admin@333", "Admin"),
            ("admin4@itsm.com", "Admin@444", "Admin")
        };

        // حسابات موظفي الجهات (4 لكل جهة). كلمات المرور محفوظة مشفّرة فقط،
        // وتُنشأ هذه الحسابات مرة واحدة إن لم تكن موجودة. غيّر كلمات المرور من «إدارة المستخدمين».
        private static readonly (string Email, string Name, string Org, string Hash)[] SeedOrgUsers =
        {
            ("modon1@itsm.com", "موظف مدن 1", "مدن", "PBKDF2$600000$9CB/KkJ6Kr86I1Kxap2rFA==$yH/4a6Oz+o0MLhqsOXLvdRlXcvGQ7HzX4Kdi034DjB0="),
            ("modon2@itsm.com", "موظف مدن 2", "مدن", "PBKDF2$600000$OQq8pmTlJETkepHGhiGS3w==$v6vCDYdckfGKKGNO8Khu8szGbr55Qa6DQVHSKqbWLuA="),
            ("modon3@itsm.com", "موظف مدن 3", "مدن", "PBKDF2$600000$XHiVIYO6QCLdcBVHPs1d0w==$a7TRXji4fuTMbOu+PT3PjdWDBxdet2vzf+E0JCqmLkM="),
            ("modon4@itsm.com", "موظف مدن 4", "مدن", "PBKDF2$600000$eOyBmzRPglBcXwN/OmikYA==$eTQxGEk2ueYgEliLyOVaw2HtvNFB93JvEnz2I7T7Wq8="),
            ("moh1@itsm.com", "موظف وزارة الصحة 1", "وزارة الصحة", "PBKDF2$600000$Ak81qHGjmEShLzMDL9oePg==$LoC/G5IDYUn8a9VlOiYZiX6KSRCpT4oOGEQcuNIz9SA="),
            ("moh2@itsm.com", "موظف وزارة الصحة 2", "وزارة الصحة", "PBKDF2$600000$twUiVPZh2qY8ax5gXwA13g==$fAU8M+O+aEvmGG0XEWs2Awoqm8CO8qCpueXs3j6G9Z4="),
            ("moh3@itsm.com", "موظف وزارة الصحة 3", "وزارة الصحة", "PBKDF2$600000$VA41cZNP4ha8XLXePJtIhw==$iW0WxrcUpRkxQOEQVkATtSrBVw6IqVQWFht1+B2WTX0="),
            ("moh4@itsm.com", "موظف وزارة الصحة 4", "وزارة الصحة", "PBKDF2$600000$bGZz+PD7/GNjOdk5oKsKXg==$SBOZidzUfTwXhgK7/Nq6ILh0OyOOLm1TYgGQxMCC16s="),
            ("monshaat1@itsm.com", "موظف منشآت 1", "منشآت", "PBKDF2$600000$dS4E0L6EafALNPVn7PubHQ==$DoHyaDZ0j0uJEZt2AGau4oRUlXnfw+bFTOQljxQTcXQ="),
            ("monshaat2@itsm.com", "موظف منشآت 2", "منشآت", "PBKDF2$600000$3KuMtUrhw4ZPWGZzCNYpJg==$j7tRR6VcGDQeYUmwKi/INl4dXRrePIhDEphfovs0Gbg="),
            ("monshaat3@itsm.com", "موظف منشآت 3", "منشآت", "PBKDF2$600000$5UIE4eWj5YSejdSodwBvjw==$utomtJmH4b8i4VIM+IuW8NV6F5JmZ1O/UR54D+EoFLE="),
            ("monshaat4@itsm.com", "موظف منشآت 4", "منشآت", "PBKDF2$600000$r4vUBBp6HknDL7iGdyP5YQ==$UvRmPOBFDK/NtWozPnfb0Ajhw37KNX0JJc1vAHPfvak="),
            ("alhayah1@itsm.com", "موظف المحمية 1", "هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية", "PBKDF2$600000$0knC2L6bpMhbAUEOAZO4Og==$2JZDQWmC2Cxj55OrNHDVoWhu/fCwPDTYpByTBIfEdUE="),
            ("alhayah2@itsm.com", "موظف المحمية 2", "هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية", "PBKDF2$600000$NPuTpCmNt4R9sm3KzvOCOg==$0w6LLHIi0NaPz69BnqHXPSynEpKdhsfK+Z8TarB0GXM="),
            ("alhayah3@itsm.com", "موظف المحمية 3", "هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية", "PBKDF2$600000$gYcChXhN+puOYCALZuOtlg==$c8+teefsIcoq0LsAIdtOp0E2v1BPNVx95dXSz/0JNRo="),
            ("alhayah4@itsm.com", "موظف المحمية 4", "هيئة تطوير محمية الإمام عبدالعزيز بن محمد الملكية", "PBKDF2$600000$s7uLv3KSUjighd4EnqCK+Q==$4FyWq17RbFbKa2uR6MTlFw4ARt5Y5w08L4QqxHABFEA=")
        };

        private static bool _orgUsersSeeded;

        // حسابات الموظفين التجريبية القديمة: استُبدلت بحسابات موظفي الجهات وتُحذف تلقائياً
        private static readonly string[] RetiredEmails =
        {
            "employee1@itsm.com", "employee2@itsm.com", "employee3@itsm.com", "employee4@itsm.com"
        };

        private async Task EnsureOrgUsersSeedAsync()
        {
            if (_orgUsersSeeded)
            {
                return;
            }

            var emails = SeedOrgUsers.Select(u => u.Email).ToList();
            var existing = await _context.Users
                .Where(u => emails.Contains(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            var orgs = await _context.Organizations.ToDictionaryAsync(o => o.Name, o => o.Id);

            foreach (var u in SeedOrgUsers)
            {
                if (existing.Contains(u.Email)) continue;
                if (!orgs.TryGetValue(u.Org, out var orgId)) continue;

                _context.Users.Add(new AppUser
                {
                    Email = u.Email,
                    FullName = u.Name,
                    Role = "Employee",
                    PasswordHash = u.Hash,
                    OrganizationId = orgId,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();

            // لا نحذف الحسابات القديمة إلا بعد التأكد من وجود حسابات موظفي الجهات
            var replacementsExist = await _context.Users.AnyAsync(u => emails.Contains(u.Email));
            if (replacementsExist)
            {
                var retired = await _context.Users.Where(u => RetiredEmails.Contains(u.Email)).ToListAsync();
                if (retired.Count > 0)
                {
                    _context.Users.RemoveRange(retired);
                    await _context.SaveChangesAsync();
                }
            }

            _orgUsersSeeded = true;
        }

        private async Task EnsureSeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return;
            }

            foreach (var (email, password, role) in SeedUsers)
            {
                _context.Users.Add(new AppUser
                {
                    Email = email,
                    FullName = email.Split('@')[0],
                    Role = role,
                    PasswordHash = PasswordHelper.Hash(password),
                    IsActive = true
                });
            }
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await EnsureSeedAsync();
            await EnsureOrgSeedAsync();
            await EnsureOrgUsersSeedAsync();

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            await EnsureSeedAsync();
            await EnsureOrgSeedAsync();
            await EnsureOrgUsersSeedAsync();

            const string genericError = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
            var cleanEmail = (email ?? string.Empty).Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == cleanEmail);

            if (user == null)
            {
                ModelState.AddModelError("", genericError);
                return View();
            }

            // قفل مؤقت بعد محاولات خاطئة متكررة
            if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.Now)
            {
                var minutes = (int)Math.Ceiling((user.LockoutUntil.Value - DateTime.Now).TotalMinutes);
                ModelState.AddModelError("", $"تم إيقاف الدخول مؤقتاً بسبب محاولات خاطئة متكررة. حاول بعد {minutes} دقيقة.");
                return View();
            }

            if (!PasswordHelper.Verify(password ?? string.Empty, user.PasswordHash))
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 5)
                {
                    user.LockoutUntil = DateTime.Now.AddMinutes(5);
                    user.FailedAttempts = 0;
                }
                await _context.SaveChangesAsync();

                ModelState.AddModelError("", genericError);
                return View();
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "هذا الحساب موقوف. تواصل مع مدير النظام.");
                return View();
            }

            // جهة الموظف وشعارها (إن وُجدت)
            string orgName = string.Empty;
            string orgLogo = string.Empty;
            if (user.OrganizationId.HasValue)
            {
                var org = await _context.Organizations.FindAsync(user.OrganizationId.Value);
                if (org != null)
                {
                    if (!org.IsActive)
                    {
                        ModelState.AddModelError("", "جهة هذا الحساب موقوفة. تواصل مع مدير النظام.");
                        return View();
                    }
                    orgName = org.Name;
                    orgLogo = org.LogoFileName;
                }
            }

            user.FailedAttempts = 0;
            user.LockoutUntil = null;
            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            HttpContext.Session.Clear();
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            if (orgName.Length > 0)
            {
                HttpContext.Session.SetString("OrgName", orgName);
                HttpContext.Session.SetString("OrgLogo", orgLogo ?? string.Empty);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            if (!PasswordHelper.Verify(currentPassword ?? string.Empty, user.PasswordHash))
            {
                ModelState.AddModelError("", "كلمة المرور الحالية غير صحيحة.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "كلمة المرور الجديدة وتأكيدها غير متطابقتين.");
                return View();
            }

            if (newPassword == currentPassword)
            {
                ModelState.AddModelError("", "يجب أن تختلف كلمة المرور الجديدة عن الحالية.");
                return View();
            }

            var strengthError = PasswordHelper.ValidateStrength(newPassword);
            if (strengthError != null)
            {
                ModelState.AddModelError("", strengthError);
                return View();
            }

            user.PasswordHash = PasswordHelper.Hash(newPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تغيير كلمة المرور بنجاح.";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}