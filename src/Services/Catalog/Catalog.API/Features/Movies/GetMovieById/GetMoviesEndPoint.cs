namespace Catalog.API.Features.Movies.GetMovieById;
public record GetMoviesRequest(int? PageNumber, int? PageSize);

public record GetMoviesResponse(IEnumerable<Movie> Movies);

public class GetMoviesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Products", async ([AsParameters] GetMoviesRequest request, ISender sender) =>
            {
                var query = request.Adapt<GetMoviesQuery>();
                var result = await sender.Send(query);
                var response = result.Adapt<GetMoviesResponse>();
                return Results.Ok(response);
            })
            .WithName("Get Movies")
            .Produces<GetMoviesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get movies")
            .WithDescription("Get movies"); 
    }
}