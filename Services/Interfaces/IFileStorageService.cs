namespace WebTerriane.Services.Interfaces
{
    /// <summary>
    /// Abstracts where files physically live. Property/Application services
    /// depend only on this interface — never on Azure.Storage.Blobs or
    /// System.IO directly — so storage can be Blob Storage in production and
    /// local disk in dev/test without any calling code changing.
    ///
    /// Two container "roles" are used across the app:
    ///   - "property-images"       -> isPrivate: false (public, CDN-fronted later)
    ///   - "application-documents" -> isPrivate: true  (SAS URL only, short expiry)
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file and returns its storage path (NOT a full URL).
        /// Callers persist this path in the DB (PropertyImage.BlobPath,
        /// ApplicationDocument.BlobPath) and resolve it to an actual URL
        /// later via GetAccessUrlAsync — this keeps the DB free of
        /// account-specific or expiring URLs.
        /// </summary>
        Task<string> UploadAsync(Stream content, string fileName, string containerName, bool isPrivate);

        Task DeleteAsync(string storagePath, string containerName);

        /// <summary>
        /// Resolves a stored path to a usable URL.
        /// Public containers: returns a stable public/CDN URL.
        /// Private containers: returns a SAS URL that expires after
        /// sasExpiry (default short window) — generated fresh on each call,
        /// never cached or stored.
        /// </summary>
        Task<string> GetAccessUrlAsync(string storagePath, string containerName, TimeSpan? sasExpiry = null);
    }
}
