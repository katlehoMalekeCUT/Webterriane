using System.ComponentModel.DataAnnotations;

namespace WebTerriane.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        public string Subject { get; set; } = "General enquiry";

        [Required(ErrorMessage = "Please enter a message.")]
        [StringLength(2000, ErrorMessage = "Message is too long.")]
        public string Message { get; set; } = string.Empty;
    }
}