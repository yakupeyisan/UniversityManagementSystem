namespace UniversityMS.Infrastructure.Configuration;

public class StorageSettings
{
    public string Type { get; set; } = "Local"; // Local, Azure, AWS, MinIO
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "university-files";
    public string BasePath { get; set; } = "wwwroot/files";
    public long MaxFileSize { get; set; } = 52428800; // 50 MB
}