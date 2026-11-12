using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itsm.Models
{
    // خدمة تعمل في الخلفية: تفحص مهل SLA كل دقيقة وترسل التنبيهات للفني والمدراء
    public class SlaMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SlaMonitorService> _logger;

        public SlaMonitorService(IServiceScopeFactory scopeFactory, ILogger<SlaMonitorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // نمهل التطبيق لحظات عند بدء التشغيل
            try { await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken); }
            catch (OperationCanceledException) { return; }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "فشل فحص مهل SLA");
                }

                try { await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }

        private static string FormatSpan(TimeSpan ts)
        {
            ts = ts.Duration();
            var days = (int)ts.TotalDays;
            return days > 0 ? $"{days} يوم {ts.Hours} ساعة" : $"{(int)ts.TotalHours} ساعة {ts.Minutes} دقيقة";
        }

        private async Task CheckAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.Now;
            var active = new[] { TicketStatus.Approved, TicketStatus.InProgress };

            var tickets = await db.Tickets
                .Where(t => active.Contains(t.Status)
                            && t.SlaStartTime != null
                            && t.SlaDeadline != null
                            && (!t.SlaWarningSent || !t.SlaBreachSent))
                .ToListAsync(ct);

            if (tickets.Count == 0)
            {
                return;
            }

            var managers = await db.Users
                .Where(u => u.IsActive && (u.Role == "Vendor" || u.Role == "Admin"))
                .Select(u => u.Email)
                .ToListAsync(ct);

            void Notify(string? email, string message, int ticketId)
            {
                if (string.IsNullOrWhiteSpace(email)) return;
                db.Notifications.Add(new Notification
                {
                    UserEmail = email.Trim().ToLowerInvariant(),
                    Message = message,
                    TicketId = ticketId,
                    CreatedAt = DateTime.Now
                });
            }

            foreach (var t in tickets)
            {
                var start = t.SlaStartTime!.Value;
                var deadline = t.SlaDeadline!.Value;
                var total = deadline - start;
                var remaining = deadline - now;

                if (remaining <= TimeSpan.Zero)
                {
                    // انتهت المهلة
                    if (!t.SlaBreachSent)
                    {
                        t.SlaBreachSent = true;
                        t.SlaWarningSent = true;

                        var msg = $"⚠️ تجاوزت التذكرة #{t.Id} مهلة SLA — ما زالت قيد المعالجة";
                        Notify(t.AssignedEngineerId, msg, t.Id);
                        foreach (var m in managers) Notify(m, msg, t.Id);

                        db.Activities.Add(new TicketActivity
                        {
                            TicketId = t.Id,
                            Kind = 0,
                            Text = "تجاوزت التذكرة مهلة SLA (ما زالت قيد المعالجة)",
                            ActorEmail = string.Empty,
                            ActorRole = "النظام",
                            CreatedAt = now
                        });
                    }
                }
                else if (!t.SlaWarningSent && remaining.TotalMinutes <= total.TotalMinutes * 0.25)
                {
                    // بقي أقل من 25% من المهلة
                    t.SlaWarningSent = true;

                    var msg = $"⏳ اقتربت مهلة SLA للتذكرة #{t.Id} — متبقي {FormatSpan(remaining)}";
                    Notify(t.AssignedEngineerId, msg, t.Id);
                    foreach (var m in managers) Notify(m, msg, t.Id);
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}