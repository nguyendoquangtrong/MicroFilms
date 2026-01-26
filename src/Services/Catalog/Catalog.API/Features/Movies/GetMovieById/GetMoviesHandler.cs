namespace Catalog.API.Features.Movies.GetMovieById;

public record GetMoviesQuery(int? PageNumber, int? PageSize) : IQuery<GetMoviesResult>;
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