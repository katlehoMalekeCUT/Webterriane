using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTerriane.Models
{
    public class PropertyImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        // Path *within* the blob container, e.g. "{guid}-room.jpg".
        // NEVER the full URL — full URL (public CDN link or SAS link) is
        // generated on demand by IFileStorageService so that a container
        // rename or SAS policy change doesn't require a data migration.
        [Required, MaxLength(500)]
        public string BlobPath { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public bool IsCoverImage { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
