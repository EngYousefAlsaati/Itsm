using ClosedXML.Excel;
using Itsm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Itsm.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TicketsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // تسجيل حدث (Kind = 0) أو تعليق (Kind = 1) في سجل التذكرة
        private void AddLog(int ticketId, string text, int kind = 0, string channel = "")
        {
            _context.Activities.Add(new TicketActivity
            {
                TicketId = ticketId,
                Kind = kind,
                Channel = channel,
                Text = text,
                ActorEmail = HttpContext.Session.GetString("UserEmail") ?? string.Empty,
                ActorRole = HttpContext.Session.GetString("UserRole") ?? string.Empty,
                CreatedAt = DateTime.Now
            });
        }

        // إرسال إشعار لمستخدم (لا يُرسل لمن قام بالإجراء نفسه)
        private void Notify(string? toEmail, string message, int? ticketId)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            var me = HttpContext.Session.GetString("UserEmail");
            if (string.Equals(toEmail.Trim(), me, StringComparison.OrdinalIgnoreCase)) return;

            _context.Notifications.Add(new Notification
            {
                UserEmail = toEmail.Trim().ToLowerInvariant(),
                Message = message,
                TicketId = ticketId,
                CreatedAt = DateTime.Now
            });
        }

        private void NotifyManagers(string message, int? ticketId)
        {
            var managers = _context.Users
                .Where(u => u.IsActive && u.Role == "Vendor")
                .Select(u => u.Email)
                .ToList();

            foreach (var m in managers)
            {
                Notify(m, message, ticketId);
            }
        }

        // قناة ظهور النشاط (تدعم التعليقات القديمة التي بلا قناة بحسب دور كاتبها)
        private static string EffectiveChannel(TicketActivity a)
        {
            if (a.Kind != 1) return a.Channel;
            if (!string.IsNullOrEmpty(a.Channel)) return a.Channel;
            return a.ActorRole == "Employee" ? "Employee" : a.ActorRole == "Technician" ? "Technician" : "Internal";
        }

        // ما يراه كل دور من سجل التذكرة والتعليقات
        private static List<TicketActivity> VisibleActivities(IEnumerable<TicketActivity> all, string? role)
        {
            return all.Where(a =>
            {
                var ch = EffectiveChannel(a);
                return role switch
                {
                    "Admin" or "Vendor" => true,
                    "Employee" => ch == "" || ch == "Employee",
                    "Technician" => ch == "" || ch == "Technician",
                    _ => false
                };
            }).ToList();
        }

        // صف اتفاقية المشروع لمستوى تأثير معيّن (أو القيمة الافتراضية إن لم يوجد)
        private async Task<ProjectSla> GetSlaAsync(int? projectId, RiskLevel level)
        {
            if (projectId.HasValue)
            {
                var row = await _context.ProjectSlas.FirstOrDefaultAsync(s => s.ProjectId == projectId.Value && s.Level == level);
                if (row != null) return row;
            }
            return SlaDefaults.Create(projectId ?? 0).First(s => s.Level == level);
        }

        // من يحق له رؤية التذكرة؟
        private static bool CanViewTicket(Ticket t, string? email, string? role)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return role switch
            {
                "Admin" or "Vendor" => true,
                "Employee" => string.Equals(t.EmployeeId, email, StringComparison.OrdinalIgnoreCase),
                "Technician" => string.Equals(t.AssignedEngineerId, email, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        // ---------- المرفقات: القيود والحفظ الآمن ----------
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 ميجابايت للملف
        private const int MaxFilesPerUpload = 5;
        private static readonly string[] AllowedExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };

        // المرفقات تُحفظ خارج المجلد العام (wwwroot) ولا تُفتح إلا عبر Download بعد التحقق من الصلاحية
        private string UploadsFolder => Path.Combine(_env.ContentRootPath, "App_Data", "uploads");

        // التأكد من أن محتوى الملف يطابق امتداده (توقيع البايتات الأولى)
        private static bool SignatureOk(IFormFile file, string ext)
        {
            if (ext == ".txt") return true;

            var head = new byte[8];
            int read;
            using (var stream = file.OpenReadStream())
            {
                read = stream.Read(head, 0, head.Length);
            }
            if (read < 4) return false;

            bool Starts(params byte[] sig)
            {
                if (read < sig.Length) return false;
                for (int i = 0; i < sig.Length; i++)
                {
                    if (head[i] != sig[i]) return false;
                }
                return true;
            }

            return ext switch
            {
                ".jpg" or ".jpeg" => Starts(0xFF, 0xD8, 0xFF),
                ".png" => Starts(0x89, 0x50, 0x4E, 0x47),
                ".gif" => Starts(0x47, 0x49, 0x46, 0x38),
                ".pdf" => Starts(0x25, 0x50, 0x44, 0x46),
                ".docx" or ".xlsx" => Starts(0x50, 0x4B),
                ".doc" or ".xls" => Starts(0xD0, 0xCF, 0x11, 0xE0),
                _ => false
            };
        }

        // يرجع نص الخطأ إن وُجد مشكلة في الملفات، أو null إذا كانت سليمة
        private static string? ValidateFiles(IEnumerable<IFormFile>? files)
        {
            if (files == null) return null;

            int count = 0;
            foreach (var f in files.Where(f => f.Length > 0))
            {
                count++;
                var name = Path.GetFileName(f.FileName);
                var ext = Path.GetExtension(name).ToLowerInvariant();

                if (!AllowedExtensions.Contains(ext))
                    return $"نوع الملف غير مسموح: {name}. المسموح: صور وPDF وWord وExcel ونص.";
                if (f.Length > MaxFileSize)
                    return $"حجم الملف يتجاوز 10 ميجابايت: {name}";
                if (!SignatureOk(f, ext))
                    return $"محتوى الملف لا يطابق امتداده: {name}";
            }

            if (count > MaxFilesPerUpload)
                return $"الحد الأقصى {MaxFilesPerUpload} ملفات في المرة الواحدة.";

            return null;
        }

        // حفظ الملفات باسم عشوائي (GUID) خارج wwwroot (لا يحفظ التغييرات في القاعدة، استدعِ SaveChangesAsync بعده)
        private async Task SaveFilesAsync(int ticketId, IEnumerable<IFormFile> files, string uploaderRole)
        {
            Directory.CreateDirectory(UploadsFolder);

            foreach (var file in files.Where(f => f.Length > 0))
            {
                var originalName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(originalName).ToLowerInvariant();
                var storedName = Guid.NewGuid().ToString("N") + ext;
                var path = Path.Combine(UploadsFolder, storedName);

                using (var stream = new FileStream(path, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream);
                }

                _context.TicketAttachments.Add(new TicketAttachment
                {
                    TicketId = ticketId,
                    FileName = storedName,
                    OriginalFileName = originalName,
                    UploadedAt = DateTime.Now,
                    UploaderRole = uploaderRole
                });
            }
        }

        // مشاريع عميل الموظف الجارية (النشطة وغير المنتهية) لقائمة اختيار المشروع
        private async Task LoadProjectsAsync(int? organizationId, int? selected)
        {
            var list = new List<SelectListItem>();
            if (organizationId.HasValue)
            {
                var today = DateTime.Today;
                var projects = await _context.Projects
                    .Where(p => p.OrganizationId == organizationId.Value && p.IsActive && p.EndDate >= today)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                list = projects
                    .Select(p => new SelectListItem($"{p.ProjectNumber} — {p.Name}", p.Id.ToString(), selected == p.Id))
                    .ToList();
            }
            ViewBag.Projects = list;
        }

        // قائمة التذاكر: بحث وفلترة وترقيم صفحات (كل مستخدم يرى ما يخصه فقط)
        public async Task<IActionResult> Index(string? q, TicketStatus? status, RiskLevel? risk, int pageNo = 1)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<Ticket> query = _context.Tickets.Include(t => t.Project);
            if (role == "Employee")
            {
                query = query.Where(t => t.EmployeeId == email);
            }
            else if (role == "Technician")
            {
                query = query.Where(t => t.AssignedEngineerId == email);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t =>
                    t.Title.Contains(q) ||
                    t.Description.Contains(q) ||
                    t.AssignedEngineerId.Contains(q) ||
                    t.EmployeeId.Contains(q) ||
                    t.Organization.Contains(q) ||
                    t.Id.ToString() == q);
            }

            if (status.HasValue)
            {
                if (role == "Employee" && status == TicketStatus.InProgress)
                {
                    query = query.Where(t => t.Status == TicketStatus.InProgress || t.Status == TicketStatus.PendingValidation);
                }
                else
                {
                    query = query.Where(t => t.Status == status.Value);
                }
            }

            if (risk.HasValue)
            {
                query = query.Where(t => t.Risk == risk.Value);
            }

            const int pageSize = 10;
            int total = await query.CountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            pageNo = Math.Clamp(pageNo, 1, totalPages);

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Q = q;
            ViewBag.Status = status;
            ViewBag.Risk = risk;
            ViewBag.Page = pageNo;
            ViewBag.TotalPages = totalPages;
            ViewBag.Total = total;

            return View(tickets);
        }

        // إنشاء تذكرة: للموظف فقط
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Employee")
            {
                return RedirectToAction(nameof(Index));
            }

            // اسم الموظف يُعبّأ مسبقاً من حسابه (ويستطيع تعديله)
            var email = HttpContext.Session.GetString("UserEmail");
            var me = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            await LoadProjectsAsync(me?.OrganizationId, null);
            var projectList = ViewBag.Projects as List<SelectListItem> ?? new List<SelectListItem>();
            ViewBag.NoProjects = projectList.Count == 0;
            return View(new Ticket { EmployeeName = me?.FullName ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(55_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 55_000_000)]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,EmployeeName,Organization,MobileNumber,EmployeeEmail")] Ticket ticket,
            int? projectId,
            List<IFormFile> attachments)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("UserRole") != "Employee")
            {
                return RedirectToAction(nameof(Index));
            }

            // اختيار المشروع إلزامي من مشاريع عميل الموظف الجارية
            var projectOwner = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            await LoadProjectsAsync(projectOwner?.OrganizationId, projectId);
            var availableProjects = ViewBag.Projects as List<SelectListItem> ?? new List<SelectListItem>();
            if (availableProjects.Count == 0)
            {
                // لا تُنشأ تذكرة لعميل ليس له مشروع جارٍ
                ViewBag.NoProjects = true;
                return View(ticket);
            }

            if (projectId.HasValue && availableProjects.Any(p => p.Value == projectId.Value.ToString()))
            {
                ticket.ProjectId = projectId;
            }
            else
            {
                ModelState.AddModelError("", "اختر المشروع الذي يخصه الطلب.");
            }

            var fileError = ValidateFiles(attachments);
            if (fileError != null)
            {
                ModelState.AddModelError("", fileError);
                return View(ticket);
            }

            // مستوى التأثير يحدده مدير الاستقبال عند قبول التذكرة
            ticket.Risk = RiskLevel.Unset;

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            ticket.EmployeeId = userEmail;

            // بيانات مقدّم الطلب تُؤخذ من حسابه تلقائياً (لا يحدّد الجهة بنفسه)
            var me = await _context.Users.Include(u => u.Organization).FirstOrDefaultAsync(u => u.Email == userEmail);
            if (me != null)
            {
                ticket.EmployeeEmail = me.Email;
                if (string.IsNullOrWhiteSpace(ticket.EmployeeName) && !string.IsNullOrWhiteSpace(me.FullName)) ticket.EmployeeName = me.FullName;
                if (me.Organization != null) ticket.Organization = me.Organization.Name;
            }
            ticket.CreatedAt = DateTime.Now;
            ticket.Status = TicketStatus.PendingApproval;
            ticket.AssignedEngineerId = string.Empty;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            AddLog(ticket.Id, "إنشاء التذكرة");
            NotifyManagers($"تذكرة جديدة #{ticket.Id}: {ticket.Title} — بانتظار قرارك", ticket.Id);
            await _context.SaveChangesAsync();

            if (attachments != null && attachments.Count > 0)
            {
                await SaveFilesAsync(ticket.Id, attachments, "Employee");
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Attachments)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (!CanViewTicket(ticket, email, role))
            {
                TempData["Error"] = "ليست لديك صلاحية لعرض هذه التذكرة.";
                return RedirectToAction("Dashboard", "Home");
            }

            var all = await _context.Activities
                .Where(a => a.TicketId == ticket.Id)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
            ViewBag.Activities = VisibleActivities(all, role);

            ViewBag.Technicians = await _context.Users
                .Where(u => u.IsActive && u.Role == "Technician")
                .OrderBy(u => u.Email)
                .Select(u => u.Email)
                .ToListAsync();

            // مستويات اتفاقية مشروع التذكرة (أو القيم الافتراضية) لنموذج قبول التذكرة
            var levels = ticket.ProjectId.HasValue
                ? await _context.ProjectSlas.Where(x => x.ProjectId == ticket.ProjectId.Value).ToListAsync()
                : new List<ProjectSla>();
            foreach (var d in SlaDefaults.Create(ticket.ProjectId ?? 0))
            {
                if (!levels.Any(l => l.Level == d.Level)) levels.Add(d);
            }
            ViewBag.SlaLevels = levels.OrderByDescending(l => l.Level).ToList();

            return View(ticket);
        }

        // فتح / تنزيل مرفق: لا يُسمح إلا لمن له صلاحية على التذكرة
        public async Task<IActionResult> Download(int id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var att = await _context.TicketAttachments.FindAsync(id);
            if (att == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(att.TicketId);
            if (ticket == null)
            {
                return NotFound();
            }

            if (!CanViewTicket(ticket, email, role))
            {
                TempData["Error"] = "ليست لديك صلاحية لفتح هذا المرفق.";
                return RedirectToAction("Dashboard", "Home");
            }

            var safeName = Path.GetFileName(att.FileName);
            var path = Path.Combine(UploadsFolder, safeName);
            if (!System.IO.File.Exists(path))
            {
                // ملفات قديمة كانت محفوظة داخل wwwroot/uploads
                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                path = Path.Combine(webRoot, "uploads", safeName);
            }
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            if (!new FileExtensionContentTypeProvider().TryGetContentType(att.OriginalFileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            Response.Headers["X-Content-Type-Options"] = "nosniff";

            // الصور وPDF تُعرض داخل المتصفح، وأي نوع آخر يُنزَّل كملف
            var inline = (contentType.StartsWith("image/") && contentType != "image/svg+xml") || contentType == "application/pdf";
            return inline
                ? PhysicalFile(path, contentType)
                : PhysicalFile(path, contentType, att.OriginalFileName);
        }

        // الاستعلام المقيّد بدور المستخدم مع البحث والفلترة (للتصدير والتقارير)
        private IQueryable<Ticket> ScopedQuery(string email, string? role, string? q, TicketStatus? status, RiskLevel? risk)
        {
            IQueryable<Ticket> query = _context.Tickets.Include(t => t.Project);
            if (role == "Employee")
            {
                query = query.Where(t => t.EmployeeId == email);
            }
            else if (role == "Technician")
            {
                query = query.Where(t => t.AssignedEngineerId == email);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t =>
                    t.Title.Contains(q) ||
                    t.Description.Contains(q) ||
                    t.AssignedEngineerId.Contains(q) ||
                    t.EmployeeId.Contains(q) ||
                    t.Organization.Contains(q) ||
                    t.Id.ToString() == q);
            }

            if (status.HasValue)
            {
                if (role == "Employee" && status == TicketStatus.InProgress)
                {
                    query = query.Where(t => t.Status == TicketStatus.InProgress || t.Status == TicketStatus.PendingValidation);
                }
                else
                {
                    query = query.Where(t => t.Status == status.Value);
                }
            }

            if (risk.HasValue)
            {
                query = query.Where(t => t.Risk == risk.Value);
            }

            return query;
        }

        // تصدير قائمة التذاكر (بحسب البحث والفلترة الحالية) إلى ملف Excel
        public async Task<IActionResult> ExportExcel(string? q, TicketStatus? status, RiskLevel? risk)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = await ScopedQuery(email, role, q, status, risk)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("التذاكر");
            ws.RightToLeft = true;

            string[] headers =
            {
                "رقم التذكرة", "العنوان", "الوصف", "الموظف", "الجهة", "الخطورة", "الحالة", "الفني",
                "تاريخ الإنشاء", "بدء SLA", "الموعد النهائي SLA", "إيقاف SLA", "نتيجة SLA",
                "تقرير الحل"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }

            var head = ws.Range(1, 1, 1, headers.Length);
            head.Style.Font.Bold = true;
            head.Style.Font.FontColor = XLColor.White;
            head.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F3B28");
            head.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            const string dateFormat = "yyyy-mm-dd hh:mm";
            int r = 2;
            foreach (var t in tickets)
            {
                ws.Cell(r, 1).Value = t.Id;
                ws.Cell(r, 2).Value = t.Title;
                ws.Cell(r, 3).Value = t.Description;
                ws.Cell(r, 4).Value = t.EmployeeId;
                ws.Cell(r, 5).Value = t.Organization;
                ws.Cell(r, 6).Value = TicketText.Risk(t.Risk);
                ws.Cell(r, 7).Value = TicketText.Status(t.Status);
                ws.Cell(r, 8).Value = t.AssignedEngineerId;

                ws.Cell(r, 9).Value = t.CreatedAt;
                ws.Cell(r, 9).Style.DateFormat.Format = dateFormat;

                if (t.SlaStartTime.HasValue && t.Status != TicketStatus.PendingApproval)
                {
                    ws.Cell(r, 10).Value = t.SlaStartTime.Value;
                    ws.Cell(r, 10).Style.DateFormat.Format = dateFormat;
                }
                if (t.SlaDeadline.HasValue && t.Status != TicketStatus.PendingApproval)
                {
                    ws.Cell(r, 11).Value = t.SlaDeadline.Value;
                    ws.Cell(r, 11).Style.DateFormat.Format = dateFormat;
                }
                var stopped = t.SlaStoppedAt ?? t.ClosedAt;
                if (t.Status == TicketStatus.Closed && stopped.HasValue)
                {
                    ws.Cell(r, 12).Value = stopped.Value;
                    ws.Cell(r, 12).Style.DateFormat.Format = dateFormat;
                }

                ws.Cell(r, 13).Value = TicketText.SlaResult(t);

                ws.Cell(r, 14).Value = t.Solution;
                r++;
            }

            foreach (var col in ws.ColumnsUsed())
            {
                col.AdjustToContents();
                if (col.Width > 45) col.Width = 45;
            }
            ws.Column(3).Style.Alignment.WrapText = true;
            ws.Column(14).Style.Alignment.WrapText = true;
            ws.SheetView.FreezeRows(1);

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            var fileName = $"tickets-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // تقرير قائمة التذاكر الجاهز للطباعة / حفظ PDF
        public async Task<IActionResult> Report(string? q, TicketStatus? status, RiskLevel? risk)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = await ScopedQuery(email, role, q, status, risk)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(q)) parts.Add($"بحث: {q.Trim()}");
            if (status.HasValue) parts.Add($"الحالة: {TicketText.Status(status.Value)}");
            if (risk.HasValue) parts.Add($"الخطورة: {TicketText.Risk(risk.Value)}");
            ViewBag.Filters = parts.Count == 0 ? "كل التذاكر" : string.Join(" | ", parts);

            return View(tickets);
        }

        // تقرير تذكرة واحدة جاهز للطباعة / حفظ PDF
        public async Task<IActionResult> PrintTicket(int? id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (!CanViewTicket(ticket, email, role))
            {
                TempData["Error"] = "ليست لديك صلاحية لعرض هذه التذكرة.";
                return RedirectToAction("Dashboard", "Home");
            }

            var allActivities = await _context.Activities
                .Where(a => a.TicketId == ticket.Id)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
            ViewBag.Activities = VisibleActivities(allActivities, role);

            return View(ticket);
        }

        // إضافة تعليق: قناة الموظف أو قناة الفني.
        // كل طرف يرى قناته فقط، ومدير الاستقبال ومدير النظام يريان القناتين.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string message, string? channel)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (!CanViewTicket(ticket, email, role))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "اكتب نص التعليق أولاً.";
                return RedirectToAction(nameof(Details), new { id });
            }

            message = message.Trim();
            if (message.Length > 1000)
            {
                message = message.Substring(0, 1000);
            }

            string ch;
            if (role == "Employee") ch = "Employee";
            else if (role == "Technician") ch = "Technician";
            else ch = channel == "Technician" ? "Technician" : "Employee";

            if (ch == "Technician" && string.IsNullOrWhiteSpace(ticket.AssignedEngineerId))
            {
                TempData["Error"] = "لا يوجد فني مُسند لهذه التذكرة بعد.";
                return RedirectToAction(nameof(Details), new { id });
            }

            AddLog(id, message, 1, ch);

            if (role == "Employee")
            {
                NotifyManagers($"تعليق جديد من الموظف على التذكرة #{id}", id);
            }
            else if (role == "Technician")
            {
                NotifyManagers($"تعليق جديد من الفني على التذكرة #{id}", id);
            }
            else if (ch == "Employee")
            {
                Notify(ticket.EmployeeId, $"رد جديد على تذكرتك #{id}", id);
            }
            else
            {
                Notify(ticket.AssignedEngineerId, $"رسالة جديدة بخصوص التذكرة #{id}", id);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // مدير الاستقبال وحده: قبول التذكرة وتحديد مستوى التأثير وتوجيه الفني (يبدأ عدّاد SLA)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string technicianEmail, int level)
        {
            if (HttpContext.Session.GetString("UserRole") != "Vendor")
            {
                TempData["Error"] = "قبول التذاكر وتوجيهها من صلاحية مدير الاستقبال فقط.";
                return RedirectToAction("Dashboard", "Home");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Status != TicketStatus.PendingApproval)
            {
                TempData["Error"] = "تم البت في هذه التذكرة مسبقاً.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (level < 1 || level > 4)
            {
                TempData["Error"] = "اختر مستوى التأثير.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var tech = (technicianEmail ?? string.Empty).Trim().ToLowerInvariant();
            if (!await _context.Users.AnyAsync(u => u.Email == tech && u.Role == "Technician" && u.IsActive))
            {
                TempData["Error"] = "اختر فنياً صالحاً.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var impact = (RiskLevel)level;
            var sla = await GetSlaAsync(ticket.ProjectId, impact);
            var now = DateTime.Now;

            ticket.Risk = impact;
            ticket.AssignedEngineerId = tech;
            ticket.Status = TicketStatus.Approved;
            ticket.DecidedBy = HttpContext.Session.GetString("UserEmail") ?? string.Empty;
            ticket.DecidedAt = now;
            ticket.ResponseDeadline = ticket.CreatedAt.AddMinutes(sla.ResponseMinutes);
            ticket.SlaStartTime = now;
            ticket.SlaDeadline = now.AddMinutes(sla.ResolutionMinutes);
            ticket.SlaStoppedAt = null;
            ticket.SlaWarningSent = false;
            ticket.SlaBreachSent = false;

            var impactAr = SlaDefaults.LevelAr(impact);
            var responseNote = now <= ticket.ResponseDeadline.Value ? "ضمن زمن الاستجابة" : "بعد انقضاء زمن الاستجابة";

            AddLog(id, $"قبول التذكرة وتحديد مستوى التأثير ({impactAr}) وتوجيهها إلى {tech} — مهلة الحل {SlaDefaults.Duration(sla.ResolutionMinutes)} وبدأ عدّاد SLA ({responseNote})");
            Notify(tech, $"تم توجيه التذكرة #{id} إليك — مستوى التأثير: {impactAr}، مهلة الحل: {SlaDefaults.Duration(sla.ResolutionMinutes)}", id);
            Notify(ticket.EmployeeId, $"تم قبول تذكرتك #{id} وتوجيهها للمختص", id);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم قبول التذكرة وتوجيه الفني، وبدأ عدّاد SLA.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // مدير الاستقبال وحده: رفض التذكرة مع ذكر السبب
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            if (HttpContext.Session.GetString("UserRole") != "Vendor")
            {
                TempData["Error"] = "رفض التذاكر من صلاحية مدير الاستقبال فقط.";
                return RedirectToAction("Dashboard", "Home");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Status != TicketStatus.PendingApproval)
            {
                TempData["Error"] = "تم البت في هذه التذكرة مسبقاً.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "سبب الرفض مطلوب.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ticket.Status = TicketStatus.Rejected;
            ticket.RejectionReason = reason.Trim();
            ticket.DecidedBy = HttpContext.Session.GetString("UserEmail") ?? string.Empty;
            ticket.DecidedAt = DateTime.Now;

            AddLog(id, "رفض التذكرة");
            Notify(ticket.EmployeeId, $"تم رفض تذكرتك #{id}. السبب: {ticket.RejectionReason}", id);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم رفض التذكرة.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // الفني: «تم حل المشكلة» ← يوقف عدّاد SLA ويرسل الحل لاعتماد مدير الاستقبال
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(55_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 55_000_000)]
        public async Task<IActionResult> Resolve(int id, string solution, string? technicianNotes, List<IFormFile> attachments)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (userRole != "Technician")
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // يجب أن يكون هو الفني المُوجَّه إليه التذكرة
            if (!string.Equals(ticket.AssignedEngineerId, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ticket.Status != TicketStatus.Approved && ticket.Status != TicketStatus.InProgress)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(solution))
            {
                TempData["Error"] = "اكتب تقرير الحل قبل الإرسال.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fileError = ValidateFiles(attachments);
            if (fileError != null)
            {
                TempData["Error"] = fileError;
                return RedirectToAction(nameof(Details), new { id });
            }

            var now = DateTime.Now;
            ticket.Solution = solution.Trim();
            ticket.TechnicianNotes = technicianNotes?.Trim() ?? string.Empty;
            ticket.Status = TicketStatus.PendingValidation;
            ticket.SlaStoppedAt = now;   // هنا يتوقف عداد SLA

            AddLog(ticket.Id, "رفع الفني تقرير الحل وأوقف عدّاد SLA — بانتظار اعتماد مدير الاستقبال", 0, "Internal");
            NotifyManagers($"الفني أرسل حل التذكرة #{ticket.Id} بانتظار اعتمادك", ticket.Id);

            if (attachments != null && attachments.Count > 0)
            {
                await SaveFilesAsync(ticket.Id, attachments, "Technician");
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم إرسال الحل إلى مدير الاستقبال للاعتماد، وتوقف عدّاد SLA.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // مدير الاستقبال: اعتماد حل الفني ← تُغلق التذكرة ويظهر للموظف «تم حل المشكلة»
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveSolution(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Vendor")
            {
                TempData["Error"] = "اعتماد الحلول من صلاحية مدير الاستقبال فقط.";
                return RedirectToAction("Dashboard", "Home");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Status != TicketStatus.PendingValidation)
            {
                TempData["Error"] = "هذه التذكرة ليست بانتظار الاعتماد.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.Now;

            AddLog(id, "اعتمد مدير الاستقبال الحل — تم حل المشكلة وأُغلقت التذكرة");
            Notify(ticket.EmployeeId, $"تم حل المشكلة في تذكرتك #{id} وإغلاقها — يمكنك الآن تقييم الفني إن كان التقييم متاحاً", id);
            Notify(ticket.AssignedEngineerId, $"اعتمد مدير الاستقبال حلّك للتذكرة #{id}", id);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم اعتماد الحل وإغلاق التذكرة.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // مدير الاستقبال: إرجاع الحل للفني مع السبب ← يعود الحالة «قيد المعالجة» ويستأنف العدّاد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnSolution(int id, string reason)
        {
            if (HttpContext.Session.GetString("UserRole") != "Vendor")
            {
                TempData["Error"] = "إرجاع الحلول من صلاحية مدير الاستقبال فقط.";
                return RedirectToAction("Dashboard", "Home");
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Status != TicketStatus.PendingValidation)
            {
                TempData["Error"] = "هذه التذكرة ليست بانتظار الاعتماد.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "اكتب سبب إرجاع الحل للفني.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ticket.Status = TicketStatus.InProgress;
            ticket.SlaStoppedAt = null;   // يستأنف العدّاد

            AddLog(id, "أُعيد الحل إلى الفني لاستكمال المعالجة", 0, "Internal");
            AddLog(id, $"أُعيد الحل للمعالجة: {reason.Trim()}", 1, "Technician");
            Notify(ticket.AssignedEngineerId, $"أعاد مدير الاستقبال حل التذكرة #{id} للمعالجة: {reason.Trim()}", id);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم إرجاع الحل إلى الفني، واستؤنف عدّاد SLA.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}