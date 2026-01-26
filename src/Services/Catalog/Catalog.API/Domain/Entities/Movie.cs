namespace Catalog.API.Domain.Entities;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<string> Genres { get; set; } = new();
    public string? PosterUrl { get; set; }
    public string? VideoUrl { get; set; }
    public VideoStatus Status { get; set; } = VideoStatus.Pending;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
