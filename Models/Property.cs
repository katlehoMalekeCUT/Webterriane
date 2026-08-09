using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTerriane.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Area { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? NearestUniversity { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DistanceFromCampusKm { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyRentFrom { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MonthlyRentTo { get; set; }

        public int AvailableRooms { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsFullyBooked { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        // Blob paths, not raw URLs — resolved to public/CDN or SAS URL at read time
        // by IFileStorageService.GetAccessUrlAsync, so the DB never stores an
        // account-specific or expiring URL.
        [MaxLength(500)]
        public string? BrochureBlobPath { get; set; }

        [MaxLength(500)]
        public string? VirtualTourUrl { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(2000)]
        public string? Amenities { get; set; } // simple comma-separated or JSON list; fine at this scale

        [MaxLength(2000)]
        public string? Rules { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Navigation
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
