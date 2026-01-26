namespace Catalog.API.Features.Movies.GetMovieById;

public record GetMovieByIdQuery(Guid Id) : IQuery<GetMovieByIdResult>, ICachedQuery
{
    public string CacheKey =>  $"GetMovies_{Id}";
    public TimeSpan? Expiration =>  TimeSpan.FromMinutes(10);
}

public record GetMovieByIdResult(Movie Movie);

internal class GetMovieByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetMovieByIdQuery,GetMovieByIdResult>
{
    public async Task<GetMovieByIdResult> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
    {
        var movie = await session.LoadAsync<Movie>(query.Id, cancellationToken);
        if (movie is null)
            throw new MovieNotFoundException(query.Id);
        return new GetMovieByIdResult(movie);
    }
}