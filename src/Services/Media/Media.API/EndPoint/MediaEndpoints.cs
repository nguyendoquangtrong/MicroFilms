using Carter;
using Media.Application.Features.ConfirmUpload;
using Media.Application.Features.CreateMediaUpload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Media.API.Endpoints;

public class MediaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/media").WithTags("Media");

        // 1. Init Upload
        group.MapPost("/upload/init", async ([FromBody] CreateMediaUploadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("InitUpload")
            .WithSummary("Initialize upload and get presigned URL");

        // 2. Confirm Upload
        group.MapPost("/upload/confirm/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new ConfirmUploadCommand(id));
                return Results.Ok(result);
            })
            .WithName("ConfirmUpload")
            .WithSummary("Confirm upload completion");
    }
}