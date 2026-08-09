using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTerriane.Models
{
    public enum DocumentType
    {
        IdCopy = 0,
        ProofOfRegistration = 1,
        NsfasLetter = 2,
        ProofOfIncome = 3
    }

    public class ApplicationDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        [Required]
        public DocumentType DocumentType { get; set; }

        // Path within the PRIVATE blob container. Access is only ever granted
        // via a short-lived SAS URL generated on an authenticated admin's
        // download click — never a permanent public link, never emailed.
        [Required, MaxLength(500)]
        public string BlobPath { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
