using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebTerriane.Models
{
    // Extends ASP.NET Core Identity's IdentityUser<int>. No student accounts
    // exist anywhere in this system — this is the ONLY user type. The first
    // admin is seeded via a migration/seed script, not self-registration;
    // there is deliberately no public registration endpoint.
    public class Administrator : IdentityUser<int>
    {
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
    }

    // Simple key-value store for admin-editable homepage/site content
    // (hero images, headline text, contact details, social links, company
    // info). One flexible table beats a dozen single-purpose ones and avoids
    // a migration every time marketing wants to change a sentence.
    public class SiteSetting
    {
        [Key]
        public int SiteSettingId { get; set; }

        [Required, MaxLength(100)]
        public string Key { get; set; } = string.Empty; // e.g. "HomeHeroHeadline", "ContactPhone", "FacebookUrl"

        [Required]
        public string Value { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; } // shown in admin UI so it's clear what each key controls

        public DateTime? UpdatedDate { get; set; }
    }

    public class Testimonial
    {
        [Key]
        public int TestimonialId { get; set; }

        [Required, MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? University { get; set; }

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public int Rating { get; set; } = 5; // 1-5

        [MaxLength(500)]
        public string? PhotoBlobPath { get; set; }

        public bool IsPublished { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    public class FAQ
    {
        [Key]
        public int FAQId { get; set; }

        [Required, MaxLength(300)]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = true;
    }

    public class ContactMessage
    {
        [Key]
        public int ContactMessageId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    }
}
