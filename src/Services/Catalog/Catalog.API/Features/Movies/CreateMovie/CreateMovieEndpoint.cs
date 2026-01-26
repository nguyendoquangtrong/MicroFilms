namespace Catalog.API.Features.Movies.CreateMovie;


public class CreateMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/movies", async (CreateMovieCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
            
                // Trả về 201 Created cùng Location Header chuẩn REST
                return Results.Created($"/movies/{result.Id}", result);
            })
            .WithName("CreateMovie")
            .Produces<CreateMovieResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new movie metadata")
            .WithDescription("Creates movie metadata and triggers async processing events.");
    }
}