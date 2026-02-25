using Amazon.S3;
using Amazon.S3.Model;
using DirectoryPlatform.Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace DirectoryPlatform.Infrastructure.Services;

public class S3MediaService : IMediaService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrl;

    public S3MediaService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWSSettings:BucketName"] ?? "directory-platform";
        _publicUrl = configuration["AWSSettings:PublicUrl"] ?? "http://localhost:9000/directory-platform";
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder)
    {
        var key = $"{folder}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request);
        return $"{_publicUrl}/{key}";
    }

    public async Task DeleteAsync(string fileUrl)
    {
        var key = fileUrl.Replace($"{_publicUrl}/", "");
        await _s3Client.DeleteObjectAsync(_bucketName, key);
    }

    public async Task<string> GetPresignedUrlAsync(string fileUrl, int expiryMinutes = 60)
    {
        var key = fileUrl.Replace($"{_publicUrl}/", "");
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
        return await Task.FromResult(_s3Client.GetPreSignedURL(request));
    }
}
