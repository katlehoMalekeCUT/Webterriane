namespace WebTerriane.Services.Interfaces
{
    public interface IReferenceNumberService
    {
        /// <summary>Generates the next reference number, e.g. "WT-2026-000001".</summary>
        Task<string> GenerateNextAsync();
    }
}
