namespace Catalog.API.Features.Movies.GetMovies;

public record GetMoviesQuery(int? PageNumber, int? PageSize) : IQuery<GetMoviesResult>, ICachedQuery
{
    public string CacheKey => $"GetMovies_{PageNumber ?? 1}_{PageSize ?? 10}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);}

public record GetMoviesResult(IEnumerable<Movie> Movies);

internal class GetMoviesQueryHandler(IDocumentSession session) : IQueryHandler<GetMoviesQuery, GetMoviesResult>
{
    public async Task<GetMoviesResult> Handle(GetMoviesQuery query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Movie>()
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);
        return new GetMoviesResult(products);
    }
}