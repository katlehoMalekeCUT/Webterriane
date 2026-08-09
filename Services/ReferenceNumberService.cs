using Microsoft.EntityFrameworkCore;
using WebTerriane.Data;
using WebTerriane.Services.Interfaces;

namespace WebTerriane.Services
{
    /// <summary>
    /// Pulls from the SQL Server SEQUENCE "ApplicationRefSeq" (defined in
    /// ApplicationDbContext.OnModelCreating). NEXT VALUE FOR is atomic at
    /// the database level — no read-then-write race, no locking needed in
    /// app code, safe under concurrent application submissions.
    ///
    /// Prefix resets by calendar year (WT-2026-, WT-2027-, ...) while the
    /// sequence itself keeps counting — acceptable since uniqueness is
    /// enforced by the ReferenceNumber unique index, not by the number
    /// portion alone. If clients need the numeric part to reset each year,
    /// swap this for a per-year sequence name instead.
    /// </summary>
    public class ReferenceNumberService : IReferenceNumberService
    {
        private readonly ApplicationDbContext _db;

        public ReferenceNumberService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateNextAsync()
        {
            var next = await _db.Database
                .SqlQuery<int>($"SELECT NEXT VALUE FOR ApplicationRefSeq AS Value")
                .SingleAsync();

            return $"WT-{DateTime.UtcNow.Year}-{next:D6}";
        }
    }
}
