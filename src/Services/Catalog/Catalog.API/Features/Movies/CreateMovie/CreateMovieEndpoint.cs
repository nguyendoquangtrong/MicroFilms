namespace Catalog.API.Features.Movies.CreateMovie;

public record CreateMovieRequest( string Title,
    string Description,
    DateTime ReleaseDate,
    List<string> Genres);

public record CreateMovieResponse(Guid Id);


public class CreateMovieEndpoint : ICarterModule
{
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/movies", async (CreateMovieRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateMovieCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateMovieResponse>();
                return Results.Created($"/movie/{response.Id}", response);

            })
            .WithName("CreateMovie")
            .Produces<CreateMovieResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new movie metadata")
            .WithDescription("Creates movie metadata and triggers async processing events.");
    }
}