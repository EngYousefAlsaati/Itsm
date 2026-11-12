using Microsoft.EntityFrameworkCore;

namespace Itsm.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketActivity> Activities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectSla> ProjectSlas { get; set; }
        public DbSet<TechnicianEvaluation> TechnicianEvaluations { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // البريد فريد لكل مستخدم، واسم العميل فريد، ورقم المشروع فريد
            modelBuilder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Organization>().HasIndex(o => o.Name).IsUnique();
            modelBuilder.Entity<Project>().HasIndex(p => p.ProjectNumber).IsUnique();
            modelBuilder.Entity<ProjectSla>().HasIndex(s => new { s.ProjectId, s.Level }).IsUnique();

            // تقييم واحد من كل مقيِّم لكل فني في كل دورة
            modelBuilder.Entity<TechnicianEvaluation>().HasIndex(e => new { e.TechnicianEmail, e.EvaluatorEmail, e.PeriodKey }).IsUnique();
        }
    }
}