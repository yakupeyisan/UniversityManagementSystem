namespace UniversityMS.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] fileContent, string fileName, string folderPath, string mimeType, CancellationToken cancellationToken);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderPath = "");
    Task DeleteFileAsync(string filePath);
    Task<Stream> DownloadFileAsync(string filePath);
    Task<List<string>> ListFilesAsync(string folderPath);
}