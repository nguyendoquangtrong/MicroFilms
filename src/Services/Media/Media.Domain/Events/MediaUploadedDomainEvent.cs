using Media.Domain.Abstractions;

namespace Media.Domain.Events;

public record MediaUploadedDomainEvent(
    Guid MediaId, 
    string Bucket, 
    string Key, 
    string ContentType, 
    long Size
) : IDomainEvent;