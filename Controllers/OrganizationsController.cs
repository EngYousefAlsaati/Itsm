using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Itsm.Controllers
{
    // إدارة العملاء وشعاراتهم: لمدير النظام فقط
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OrganizationsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private const long MaxLogoSize = 2 * 1024 * 1024; // 2 ميجابايت
        private static readonly string[] LogoExtensions = { ".png", ".jpg", ".jpeg", ".webp" };

        private string LogoFolder =>
            Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "images", "orgs");

        private IActionResult? Guard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
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

        // يرجع نص الخطأ إن كان الشعار غير صالح، أو null إن كان سليماً أو غير موجود
        private static string? ValidateLogo(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!LogoExtensions.Contains(ext))
                return "صيغة الشعار غير مسموحة. المسموح: PNG أو JPG أو WEBP.";
            if (file.Length > MaxLogoSize)
                return "حجم الشعار يتجاوز 2 ميجابايت.";

            var head = new byte[12];
            int read;
            using (var stream = file.OpenReadStream())
            {
                read = stream.Read(head, 0, head.Length);
            }
            if (read < 12) return "ملف الشعار غير صالح.";

            bool ok = ext switch
            {
                ".png" => head[0] == 0x89 && head[1] == 0x50 && head[2] == 0x4E && head[3] == 0x47,
                ".jpg" or ".jpeg" => head[0] == 0xFF && head[1] == 0xD8 && head[2] == 0xFF,
                ".webp" => head[0] == 0x52 && head[1] == 0x49 && head[2] == 0x46 && head[3] == 0x46
                           && head[8] == 0x57 && head[9] == 0x45 && head[10] == 0x42 && head[11] == 0x50,
                _ => false
            };

            return ok ? null : "محتوى الملف لا يطابق امتداده.";
        }

        private async Task<string> SaveLogoAsync(IFormFile file)
        {
            Directory.CreateDirectory(LogoFolder);
            var name = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
            using var stream = new FileStream(Path.Combine(LogoFolder, name), FileMode.CreateNew);
            await file.CopyToAsync(stream);
            return name;
        }

        private void DeleteLogo(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var path = Path.Combine(LogoFolder, Path.GetFileName(fileName));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public async Task<IActionResult> Index()
        {
            var guard = Guard();
            if (guard != null) return guard;

            var orgs = await _context.Organizations.OrderBy(o => o.Name).ToListAsync();

            var counts = await _context.Users
                .Where(u => u.OrganizationId != null)
                .GroupBy(u => u.OrganizationId!.Value)
                .Select(g => new { g.Key, N = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.N);

            ViewBag.Counts = counts;

            ViewBag.ProjectCounts = await _context.Projects
                .GroupBy(p => p.OrganizationId)
                .Select(g => new { g.Key, N = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.N);
            return View(orgs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string? address, string? city, IFormFile? logo)
        {
            var guard = Guard();
            if (guard != null) return guard;

            name = (name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                TempData["Error"] = "اسم العميل مطلوب.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Organizations.AnyAsync(o => o.Name == name))
            {
                TempData["Error"] = "هذا العميل مسجّل مسبقاً.";
                return RedirectToAction(nameof(Index));
            }

            var logoError = ValidateLogo(logo);
            if (logoError != null)
            {
                TempData["Error"] = logoError;
                return RedirectToAction(nameof(Index));
            }

            var org = new Organization
            {
                Name = name,
                Address = (address ?? string.Empty).Trim(),
                City = (city ?? string.Empty).Trim()
            };
            if (logo != null && logo.Length > 0)
            {
                org.LogoFileName = await SaveLogoAsync(logo);
            }

            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تمت إضافة العميل «{name}».";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, string name, string? address, string? city, IFormFile? logo)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var org = await _context.Organizations.FindAsync(id);
            if (org == null)
            {
                return NotFound();
            }

            name = (name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                TempData["Error"] = "اسم العميل مطلوب.";
                return RedirectToAction(nameof(Index));
            }

            if (await _context.Organizations.AnyAsync(o => o.Name == name && o.Id != id))
            {
                TempData["Error"] = "يوجد عميل آخر بنفس الاسم.";
                return RedirectToAction(nameof(Index));
            }

            var logoError = ValidateLogo(logo);
            if (logoError != null)
            {
                TempData["Error"] = logoError;
                return RedirectToAction(nameof(Index));
            }

            org.Name = name;
            org.Address = (address ?? string.Empty).Trim();
            org.City = (city ?? string.Empty).Trim();
            if (logo != null && logo.Length > 0)
            {
                var old = org.LogoFileName;
                org.LogoFileName = await SaveLogoAsync(logo);
                DeleteLogo(old);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حفظ بيانات العميل.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id, bool active)
        {
            var guard = Guard();
            if (guard != null) return guard;

            var org = await _context.Organizations.FindAsync(id);
            if (org == null)
            {
                return NotFound();
            }

            org.IsActive = active;
            await _context.SaveChangesAsync();

            TempData["Success"] = active ? $"تم تفعيل العميل «{org.Name}»." : $"تم إيقاف العميل «{org.Name}» ولن يستطيع موظفوه الدخول.";
            return RedirectToAction(nameof(Index));
        }
    }
}