using Media.Domain.ValueObjects;

namespace Media.Application.Abstractions;

public interface IStorageService
{
    Task<string> GetPresignedUrlAsync(StorageLocation location, string contentType, TimeSpan expiry);
    Task<FileMetadata?> GetFileMetadataAsync(StorageLocation location);
    Task DeleteFileAsync(StorageLocation location);
}