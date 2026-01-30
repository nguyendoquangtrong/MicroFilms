using Media.Domain.Abstractions;
using Media.Domain.ValueObjects;

namespace Media.Domain.Events;

public record MediaUploadedDomainEvent(
    Guid MediaId, 
    Guid  MovieId,
    string Bucket, 
    string Key, 
    string ContentType, 
    long Size
) : IDomainEvent;