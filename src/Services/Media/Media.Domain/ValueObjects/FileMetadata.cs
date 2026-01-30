using Media.Domain.Exceptions;

namespace Media.Domain.ValueObjects;

public record FileMetadata
{
    public string FileName { get; }
    public string ContentType { get; } 
    public string Extension { get; }
    public long SizeBytes { get; private set; }

    public FileMetadata()
    {
    }
    
    private FileMetadata(string fileName, string contentType, long sizeBytes)
    {
        FileName = fileName;
        ContentType = contentType;
        Extension = Path.GetExtension(fileName).ToLowerInvariant();
        SizeBytes = sizeBytes;
    }
    
    public static FileMetadata of(string fileName, string contentType, long sizeBytes = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentOutOfRangeException.ThrowIfLessThan(sizeBytes,0);
        return new FileMetadata(fileName, contentType, sizeBytes);
    }
    
    public FileMetadata UpdateSize(long newSize)
    {
        if (newSize <= 0) 
            throw new DomainException("Invalid file size update.");
            
        return this with { SizeBytes = newSize };
    }
}