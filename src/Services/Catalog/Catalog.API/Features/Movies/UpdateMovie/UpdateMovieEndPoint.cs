namespace Catalog.API.Features.Movies.UpdateMovie;

public record UpdateMovieRequest(
    Guid Id,
    string Title,
    string Description,
    string? PosterUrl,
    string? VideoUrl,
    DateTime ReleaseDate,
    List<string> Genres,
    VideoStatus Status
);

public record UpdateMovieResponse(bool IsSuccess);

public class UpdateMovieEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/movies", async (UpdateMovieRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateMovieCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateMovieResponse>();
            return Results.Ok(response);
        }).WithName("UpdateMovie")
        .Produces<UpdateMovieResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update Movie")
        .WithDescription("Update movie metadata.");
    }
}