using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Infrastructure.Configuration;

namespace UniversityMS.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly StorageSettings _settings;
    private readonly ILogger<LocalFileStorageService> _logger;

    // Allowed MIME types (security)
    private static readonly HashSet<string> AllowedMimeTypes = new()
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain",
        "text/csv",
        "application/zip"
    };

    // Allowed file extensions
    private static readonly HashSet<string> AllowedExtensions = new()
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp",
        ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip"
    };

    public LocalFileStorageService(IOptions<StorageSettings> settings,
        ILogger<LocalFileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Dosyayı byte array'den kaydeder
    /// </summary>
    /// <param name="fileContent">Dosya içeriği (bytes)</param>
    /// <param name="fileName">Orijinal dosya adı</param>
    /// <param name="folderPath">Kaydedilecek klasör yolu</param>
    /// <param name="mimeType">Dosya MIME tipi (image/jpeg, application/pdf, vb.)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Kaydedilen dosyanın relatif yolu</returns>
    public async Task<string> SaveFileAsync(byte[] fileContent, string fileName,
        string folderPath, string mimeType, CancellationToken cancellationToken = default)
    {
        try
        {
            // ===== 1. VALIDASYON =====

            // 1.1 - Dosya içeriği kontrol
            if (fileContent == null || fileContent.Length == 0)
            {
                _logger.LogWarning("Attempting to save empty file: {FileName}", fileName);
                throw new ArgumentException("Dosya içeriği boş olamaz", nameof(fileContent));
            }

            // 1.2 - Dosya boyutu kontrol
            if (fileContent.Length > _settings.MaxFileSize)
            {
                _logger.LogWarning(
                    "File size exceeds limit: {FileName} ({Size}MB > {MaxSize}MB)",
                    fileName,
                    fileContent.Length / (1024 * 1024),
                    _settings.MaxFileSize / (1024 * 1024));

                throw new InvalidOperationException(
                    $"Dosya boyutu {_settings.MaxFileSize / (1024 * 1024)}MB'ı geçemez");
            }

            // 1.3 - MIME type kontrol (güvenlik)
            if (!AllowedMimeTypes.Contains(mimeType))
            {
                _logger.LogWarning(
                    "Unsupported MIME type: {FileName} ({MimeType})",
                    fileName, mimeType);

                throw new InvalidOperationException(
                    $"Dosya türü desteklenmiyor: {mimeType}");
            }

            // 1.4 - Dosya uzantısı kontrol
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning(
                    "Unsupported file extension: {FileName} ({Extension})",
                    fileName, fileExtension);

                throw new InvalidOperationException(
                    $"Dosya uzantısı desteklenmiyor: {fileExtension}");
            }

            // ===== 2. KLASÖR HAZIRLA =====

            // 2.1 - Tam klasör yolunu oluştur
            var directory = Path.Combine(_settings.BasePath, folderPath);

            // 2.2 - Klasörün varlığını kontrol et ve oluştur
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Created directory: {Directory}", directory);
            }

            // ===== 3. UNIQUE DOSYA ADLI OLUŞTUR =====

            // 3.1 - Collision'u önlemek için GUID ile unique ad oluştur
            var uniqueFileName = $"{Guid.NewGuid()}_{SanitizeFileName(fileName)}";

            // 3.2 - Tam dosya yolunu oluştur
            var fullFilePath = Path.Combine(directory, uniqueFileName);

            _logger.LogInformation(
                "Saving file: {FileName} -> {FullPath}",
                fileName, fullFilePath);

            // ===== 4. DOSYAYI KAYDET =====

            // 4.1 - Async olarak dosyayı yaz
            await using (var fileStream = new FileStream(
                             fullFilePath,
                             FileMode.Create,
                             FileAccess.Write,
                             FileShare.None,
                             bufferSize: 8192,
                             useAsync: true))
            {
                await fileStream.WriteAsync(fileContent, 0, fileContent.Length, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }

            // ===== 5. RELATIF YOLU OLUŞTUR =====

            // Kaydedilen dosyanın relatif yolunu oluştur
            var relativePath = Path.Combine(folderPath, uniqueFileName)
                .Replace("\\", "/"); // Linux compatibility

            // ===== 6. LOGGING =====

            _logger.LogInformation(
                "File saved successfully - OriginalName: {OriginalName}, " +
                "SavedName: {SavedName}, Size: {Size}KB, MimeType: {MimeType}, RelativePath: {RelativePath}",
                fileName,
                uniqueFileName,
                fileContent.Length / 1024,
                mimeType,
                relativePath);

            return relativePath;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("File save operation cancelled: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file: {FileName}", fileName);
            throw;
        }
    }

    /// <summary>
    /// Stream'den dosya kaydeder
    /// </summary>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName,
        string folderPath = "")
    {
        try
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty", nameof(fileStream));

            // Stream'i byte array'e dönüştür
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();

            // MIME type'ı dosya uzantısından belirle
            var mimeType = GetMimeType(fileName);

            // SaveFileAsync'ı çağır
            return await SaveFileAsync(fileContent, fileName, folderPath, mimeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            throw;
        }
    }

    /// <summary>
    /// Dosyayı siler
    /// </summary>
    public Task DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_settings.BasePath, filePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return Task.CompletedTask;
            }

            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// Dosyayı indirir (Stream olarak)
    /// </summary>
    public async Task<Stream> DownloadFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_settings.BasePath, filePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found for download: {FilePath}", filePath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            _logger.LogInformation("Starting file download: {FilePath}", filePath);

            // Dosyayı okumak için FileStream açılı bırak
            var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 8192,
                useAsync: true);

            return await Task.FromResult(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file: {FilePath}", filePath);
            throw;
        }
    }

    /// <summary>
    /// Klasördeki tüm dosyaları listeler
    /// </summary>
    public Task<List<string>> ListFilesAsync(string folderPath)
    {
        try
        {
            var directory = Path.Combine(_settings.BasePath, folderPath);

            if (!Directory.Exists(directory))
            {
                _logger.LogWarning("Directory not found: {FolderPath}", folderPath);
                return Task.FromResult(new List<string>());
            }

            var files = Directory.GetFiles(directory)
                .Select(f => Path.GetFileName(f))
                .ToList();

            _logger.LogInformation("Listed {Count} files in folder: {FolderPath}",
                files.Count, folderPath);

            return Task.FromResult(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list files: {FolderPath}", folderPath);
            throw;
        }
    }

    // ===== HELPER METHODS =====

    /// <summary>
    /// Dosya adını güvenli hale getir (illegal characters'ları kaldır)
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        // Illegal characters'ları kaldır
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        // Boş dosya adını önle
        return string.IsNullOrEmpty(sanitized) ? "file" : sanitized;
    }

    /// <summary>
    /// Dosya uzantısından MIME type'ı belirle
    /// </summary>
    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}