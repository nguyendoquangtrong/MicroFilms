namespace Media.Domain.ValueObjects;

public class StorageLocation
{
    public string Bucket { get; }
    public string Key { get; }
    public string FullPath => $"{Bucket}/{Key}";

    public StorageLocation()
    {
    }
    
    private StorageLocation(string bucket, string key)
    {
        Bucket = bucket;
        Key = key;
    }
    
    public static StorageLocation Of(string bucket, string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucket);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        return new StorageLocation(bucket, key);
    }
}