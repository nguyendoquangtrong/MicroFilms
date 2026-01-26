using System.Text.Json;
using BuildingBlocks.CQRS;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache, 
    ILogger<CachingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery, IRequest<TResponse> // Chỉ áp dụng cho ICachedQuery
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Cache
        var cachedResponse = await cache.GetStringAsync(request.CacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            logger.LogInformation("[CACHE HIT] Fetched {Key} from Redis", request.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        // 2. Nếu Cache Miss -> Gọi Database Handler
        logger.LogInformation("[CACHE MISS] {Key} not found in Redis, fetching from Database", request.CacheKey);
        var response = await next();

        // 3. Lưu kết quả vào Redis
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = request.Expiration ?? TimeSpan.FromMinutes(5) // Default 5 phút
        };

        var serializedData = JsonSerializer.Serialize(response);
        await cache.SetStringAsync(request.CacheKey, serializedData, options, cancellationToken);

        return response;
    }
}