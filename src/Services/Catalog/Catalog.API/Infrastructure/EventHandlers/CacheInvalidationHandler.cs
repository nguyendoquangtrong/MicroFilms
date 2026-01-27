namespace Catalog.API.Infrastructure.EventHandlers;

public class CacheInvalidationHandler(
    IDistributedCache cache, 
    ILogger<CacheInvalidationHandler> logger
) : 
    INotificationHandler<MovieUpdatedEvent>,
    INotificationHandler<MovieDeletedEvent>
{
    public async Task Handle(MovieUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await RemoveCacheAsync(notification.Id, cancellationToken);
        logger.LogInformation("Domain Event handled: Cache cleared for Movie {Id} (Update)", notification.Id);
    }

    public async Task Handle(MovieDeletedEvent notification, CancellationToken cancellationToken)
    {
        await RemoveCacheAsync(notification.Id, cancellationToken);
        logger.LogInformation("Domain Event handled: Cache cleared for Movie {Id} (Delete)", notification.Id);
    }

    private async Task RemoveCacheAsync(Guid movieId, CancellationToken token)
    {
        // Lưu ý: Key phải khớp logic trong GetMovieByIdHandler
        var cacheKey = $"GetMovieById_{movieId}"; 
        await cache.RemoveAsync(cacheKey, token);
        
        // TODO (Nâng cao): xóa cache của List (GetMovies), 
    }
}