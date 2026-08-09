using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using WebTerriane.Services.Interfaces;

namespace WebTerriane.Services
{
    /// <summary>
    /// Production implementation. Requires NuGet package: Azure.Storage.Blobs.
    /// Connection string comes from configuration (Key Vault-backed in prod),
    /// never hardcoded — see appsettings.json "AzureBlobStorage:ConnectionString".
    /// </summary>
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, ILogger<AzureBlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<string> UploadAsync(Stream content, string fileName, string containerName, bool isPrivate)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(
                publicAccessType: isPrivate
                    ? Azure.Storage.Blobs.Models.PublicAccessType.None
                    : Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            // GUID-prefixed name avoids collisions and prevents guessing
            // another applicant's document path.
            var uniqueName = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
            var blobClient = containerClient.GetBlobClient(uniqueName);

            await blobClient.UploadAsync(content, overwrite: false);

            _logger.LogInformation("Uploaded blob {BlobName} to container {Container}", uniqueName, containerName);

            // Store only the blob name (path within container) — the caller
            // persists this, resolving to a full URL happens on read.
            return uniqueName;
        }

        public async Task DeleteAsync(string storagePath, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(storagePath);
            await blobClient.DeleteIfExistsAsync();
        }

        public Task<string> GetAccessUrlAsync(string storagePath, string containerName, TimeSpan? sasExpiry = null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(storagePath);

            var isPrivateContainer = containerName.Equals("application-documents", StringComparison.OrdinalIgnoreCase);

            if (!isPrivateContainer)
            {
                // Public container — stable URL, safe to render directly in
                // property gallery <img> tags.
                return Task.FromResult(blobClient.Uri.ToString());
            }

            if (!blobClient.CanGenerateSasUri)
            {
                throw new InvalidOperationException(
                    "Blob client cannot generate SAS URIs. Ensure the app is configured with a " +
                    "storage account key or user delegation key, not just a SAS/anonymous connection.");
            }

            var expiry = sasExpiry ?? TimeSpan.FromMinutes(15);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = storagePath,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            _logger.LogInformation(
                "Generated {Minutes}-minute SAS URL for private blob {BlobName}",
                expiry.TotalMinutes, storagePath);

            return Task.FromResult(sasUri.ToString());
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var clean = new string(fileName.Where(c => !invalid.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(clean) ? "file" : clean;
        }
    }
}
