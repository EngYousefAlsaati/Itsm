using System.ComponentModel.DataAnnotations;

namespace Itsm.Models
{
    // عميل (جهة طالبة للخدمة) وشعاره
    public class Organization
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        // اسم ملف الشعار داخل wwwroot/images/orgs
        [StringLength(300)]
        public string LogoFileName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}