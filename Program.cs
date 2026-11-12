using Itsm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. ربط قاعدة البيانات
//    الأفضل وضع سلسلة الاتصال في appsettings.json (أو متغير بيئة في الخادم).
//    في بيئة التطوير فقط نستخدم قاعدة LocalDB الافتراضية إن لم تُضبط.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    if (builder.Environment.IsDevelopment())
    {
        connectionString = "Server=(localdb)\\mssqllocaldb;Database=ItsmDb;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
    else
    {
        throw new InvalidOperationException("لم تُضبط سلسلة الاتصال DefaultConnection في إعدادات التطبيق.");
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. الجلسات (تسجيل دخول المستخدم ودوره) بكوكيز محمية
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".Itsm.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS فقط
});

// 3. حماية النماذج من التزوير (CSRF)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".Itsm.Antiforgery";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 4. خدمات MVC
builder.Services.AddControllersWithViews();

// 5. خدمة تنبيهات SLA في الخلفية
builder.Services.AddHostedService<SlaMonitorService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ترويسات أمان عامة
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseStaticFiles();

app.UseRouting();

// تفعيل الـ Session قبل التوثيق والتوجيه
app.UseSession();

app.UseAuthorization();

// مسار البداية الافتراضي: صفحة تسجيل الدخول
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();