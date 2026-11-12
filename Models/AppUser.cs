using System.ComponentModel.DataAnnotations;

namespace Itsm.Models
{
    // مستخدم النظام (كلمة المرور مخزّنة مشفّرة ولا يمكن استرجاعها)
    public class AppUser
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        // Employee | Technician | Vendor | Admin
        [Required, StringLength(30)]
        public string Role { get; set; } = "Employee";

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // جهة الموظف (اختيارية، وتخص الموظفين طالبي الخدمة)
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        // حماية من تخمين كلمة المرور
        public int FailedAttempts { get; set; }
        public DateTime? LockoutUntil { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }
    }
}