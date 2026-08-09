namespace WebTerriane.Models
{
    // TEMP: in-memory data store so Accommodation and Application controllers
    // share the same listings instead of each hardcoding their own copy.
    // Replace with a real database-backed repository once EF Core is wired up.
    public static class ListingRepository
    {
        private static readonly List<Listing> _listings = new()
        {
            new Listing { Id = 1, Title = "Eureka Student Residence", Location = "Bloemfontein", MonthlyPrice = 3800 },
            new Listing { Id = 2, Title = "Sonelca student accommodation",      Location = "Bloemfontein", MonthlyPrice = 4200 },
            new Listing { Id = 3, Title = "Soete inval Residence",     Location = "Bloemfontein", MonthlyPrice = 3500 },
        };

        public static List<Listing> GetAll() => _listings;

        public static Listing? GetById(int id) => _listings.FirstOrDefault(l => l.Id == id);
    }
}