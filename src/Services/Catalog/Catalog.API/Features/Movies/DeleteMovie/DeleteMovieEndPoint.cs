namespace Catalog.API.Features.Movies.DeleteMovie;
public record DeleteMovieResponse(bool IsSuccess);

public class DeleteMovieEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/movies/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMovieCommand(id));
            var response = result.Adapt<DeleteMovieResponse>();
            return Results.Ok(response);
        }).WithName("DeleteMovie")
        .Produces<DeleteMovieResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Movie")
        .WithDescription("Delete a movie by Id");
    }
}