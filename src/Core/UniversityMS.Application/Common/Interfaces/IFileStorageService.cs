namespace UniversityMS.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] fileContent, string fileName, string folderPath, string mimeType, CancellationToken cancellationToken);
}