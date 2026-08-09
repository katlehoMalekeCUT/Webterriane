using System.ComponentModel.DataAnnotations;

namespace WebTerriane.Models
{
    public class ApplicationFormModel
    {
        [Required]
        public int ListingId { get; set; }

        // Carried through so the confirmation/receipt can show what they applied for,
        // without a second database lookup on POST.
        public string ListingTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your student number.")]
        [StringLength(50)]
        public string StudentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your institution.")]
        [StringLength(150)]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a preferred move-in date.")]
        [DataType(DataType.Date)]
        public DateTime MoveInDate { get; set; }

        [StringLength(1000, ErrorMessage = "Message is too long.")]
        public string? AdditionalInfo { get; set; }
    }
}