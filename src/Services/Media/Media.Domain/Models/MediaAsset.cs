using Media.Domain.Abstractions;
using Media.Domain.Enums;
using Media.Domain.Events;
using Media.Domain.Exceptions;
using Media.Domain.ValueObjects;

namespace Media.Domain.Models;


public class MediaAsset : Aggregate<Guid>
{
    public StorageLocation Storage { get; private set; } = default!;
    public FileMetadata Metadata { get; private set; } = default!;
    public MediaStatus Status { get; private set; }
    public MediaType Type { get; private set; }
    
    private MediaAsset() { }
    
    public static MediaAsset Create(Guid id, string fileName, string contentType, string bucketName, MediaType type = MediaType.Video)
    {
        // Tạo Storage Key convention: {type}/{Date}/{Id}{ext}
        var ext = Path.GetExtension(fileName);
        var folder = type.ToString().ToLower();
        var key = $"{folder}/{DateTime.UtcNow:yyyy-MM-dd}/{id}{ext}";

        var asset = new MediaAsset
        {
            Id = id,
            Status = MediaStatus.Pending,
            Type = type,
            Storage = StorageLocation.Of(bucketName, key),
            Metadata = FileMetadata.of(fileName, contentType, 0),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        return asset;
    }
    
    public void ConfirmUpload(long actualSizeOnDisk)
    {
        if (Status == MediaStatus.Uploaded) return; 
        
        if (Status != MediaStatus.Pending) 
        {
             throw new DomainException("Only pending assets can be confirmed.");
        }

        // Cập nhật size thật
        Metadata = Metadata.UpdateSize(actualSizeOnDisk);
        
        Status = MediaStatus.Uploaded;
        LastModified = DateTime.UtcNow;

        // Thêm Domain Event
        AddDomainEvent(new MediaUploadedDomainEvent(
            Id, 
            Storage.Bucket, 
            Storage.Key, 
            Metadata.ContentType, 
            Metadata.SizeBytes
        ));
    }

    public void MarkAsProcessing()
    {
        if (Status != MediaStatus.Uploaded)
            throw new DomainException("Asset must be uploaded before processing.");
            
        Status = MediaStatus.Processing;
        LastModified = DateTime.UtcNow;
    }

    public void MarkAsReady()
    {
        Status = MediaStatus.Ready;
        LastModified = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = MediaStatus.Failed;
        LastModified = DateTime.UtcNow;
    }
}