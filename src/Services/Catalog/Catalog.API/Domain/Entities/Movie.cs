namespace Catalog.API.Domain.Entities;

public class Movie
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? PosterUrl { get; set; }
    public VideoStatus Status { get; set; } = VideoStatus.Pending;
    // Chuẩn bị cho pgvector (Semantic Search)
    // Lưu ý: Cần cài extension vector trong PostgreSQL
    // public float[]? Embedding { get; set; }
}
