using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebTerriane.Models;

namespace WebTerriane.Data
{
    // Inherits IdentityDbContext for Administrator login only — no student
    // identity tables are used anywhere in this schema.
    public class ApplicationDbContext : IdentityDbContext<Administrator, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
        public DbSet<Testimonial> Testimonials => Set<Testimonial>();
        public DbSet<FAQ> FAQs => Set<FAQ>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Property -> PropertyImages: delete images when property is
            // deleted. The blob files themselves are removed by
            // PropertyService BEFORE this cascade fires (see note there) —
            // cascading the DB rows alone would leave orphaned blobs.
            builder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not cascade: deleting a property with existing
            // applications should fail loudly (or be blocked in the service
            // layer) rather than silently wipe application history.
            builder.Entity<Application>()
                .HasOne(a => a.Property)
                .WithMany(p => p.Applications)
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Application>()
                .HasMany(a => a.Documents)
                .WithOne(d => d.Application)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Application>()
                .HasIndex(a => a.ReferenceNumber)
                .IsUnique();

            builder.Entity<SiteSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            builder.Entity<Property>()
                .HasIndex(p => p.Area);

            builder.Entity<Property>()
                .HasIndex(p => p.IsFeatured);

            // SQL Server SEQUENCE backing reference-number generation.
            // See Services/ReferenceNumberService.cs — this is what makes
            // "WT-2026-000001" collision-proof under concurrent submissions
            // without app-level locking.
            builder.HasSequence<int>("ApplicationRefSeq")
                .StartsAt(1)
                .IncrementsBy(1);
        }
    }
}
