namespace DirectoryPlatform.Contracts.Services;

public interface IMediaService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder);
    Task DeleteAsync(string fileUrl);
    Task<string> GetPresignedUrlAsync(string fileUrl, int expiryMinutes = 60);
}
