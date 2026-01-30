using Amazon.S3;
using Amazon.S3.Model;
using Media.Application.Abstractions;
using Media.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Media.Infrastructure.Storage;

public class S3StorageService(IAmazonS3 s3Client, IConfiguration configuration, ILogger<S3StorageService> logger) 
    : IStorageService
{
    private readonly string _bucketName = configuration["Storage:BucketName"]!;

    public async Task<string> GetPresignedUrlAsync(StorageLocation location, string contentType, TimeSpan expiry)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = location.Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(expiry),
            ContentType = contentType
        };
        return await s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<FileMetadata?> GetFileMetadataAsync(StorageLocation location)
    {
        try
        {
            var response = await s3Client.GetObjectMetadataAsync(_bucketName, location.Key);
            return FileMetadata.of(Path.GetFileName(location.Key), response.Headers.ContentType, response.ContentLength);
        }
        catch (AmazonS3Exception)
        {
            return null;
        }
    }

    public async Task DeleteFileAsync(StorageLocation location)
    {
        await s3Client.DeleteObjectAsync(_bucketName, location.Key);
    }
}