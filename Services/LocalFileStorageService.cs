using WebTerriane.Services.Interfaces;

namespace WebTerriane.Services
{
    /// <summary>
    /// Dev-only fallback so you're not fighting Azure auth/emulator setup on
    /// a laptop. NEVER wire this up in production — it cannot honor the
    /// "private, SAS-only" contract for application-documents; everything it
    /// writes lands under wwwroot and is publicly servable by the static
    /// file middleware. Registered only when Storage:Provider == "Local" in
    /// configuration (see Program.cs).
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
        {
            _webRootPath = env.WebRootPath;
            _logger = logger;
        }

        public async Task<string> UploadAsync(Stream content, string fileName, string containerName, bool isPrivate)
        {
            if (isPrivate)
            {
                _logger.LogWarning(
                    "LocalFileStorageService cannot store {FileName} privately — it will be publicly " +
                    "servable under wwwroot. This is acceptable for local dev only, never production.",
                    fileName);
            }

            var uniqueName = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
            var folder = Path.Combine(_webRootPath, "uploads", containerName);
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, uniqueName);
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await content.CopyToAsync(fileStream);

            return uniqueName;
        }

        public Task DeleteAsync(string storagePath, string containerName)
        {
            var fullPath = Path.Combine(_webRootPath, "uploads", containerName, storagePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public Task<string> GetAccessUrlAsync(string storagePath, string containerName, TimeSpan? sasExpiry = null)
        {
            // No real access control locally — matches dev-only intent above.
            return Task.FromResult($"/uploads/{containerName}/{storagePath}");
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var clean = new string(fileName.Where(c => !invalid.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(clean) ? "file" : clean;
        }
    }
}
