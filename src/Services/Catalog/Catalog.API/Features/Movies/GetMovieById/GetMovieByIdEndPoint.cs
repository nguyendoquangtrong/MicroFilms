namespace Catalog.API.Features.Movies.GetMovieById;

public record GetMovieByIdResponse(Movie Movie);

public class GetMovieByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/movies/{id}", async (Guid id, ISender sender) =>
        {
            var result =  await sender.Send(new GetMovieByIdQuery(id));
            var response = result.Adapt<GetMovieByIdResponse>();
            return Results.Ok(response);
        })
        .WithName("Get Movies By Id")
        .Produces<GetMovieByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get movie by id")
        .WithDescription("Get movie by id");
    }
}