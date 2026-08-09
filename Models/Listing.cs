namespace WebTerriane.Models
{
    // Placeholder listing model. Swap this out once accommodation listings
    // are backed by a real database (EF Core entity) — the shape here is
    // just enough to support the Apply flow for now.
    public class Listing
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
    }
}