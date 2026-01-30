using BuildingBlocks.CQRS;
using FluentValidation;
using Media.Application.Abstractions;
using Media.Application.Data;
using Media.Domain.Enums;
using Media.Domain.Models;
using Media.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;


namespace Media.Application.Features.CreateMediaUpload;

public record CreateMediaUploadCommand(
    Guid RefId, // MovieId
    string FileName,
    string ContentType,
    MediaType Type
) : ICommand<CreateMediaUploadResult>;

public record CreateMediaUploadResult(Guid MediaId, string UploadUrl);

public class CreateMediaUploadValidator : AbstractValidator<CreateMediaUploadCommand>
{
    public CreateMediaUploadValidator()
    {
        RuleFor(x => x.RefId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
    }
}

public class CreateMediaUploadHandler(
    IApplicationDbontext dbContext,
    IStorageService storageService,
    IConfiguration config
) : ICommandHandler<CreateMediaUploadCommand, CreateMediaUploadResult>
{
    public async Task<CreateMediaUploadResult> Handle(CreateMediaUploadCommand command, CancellationToken cancellationToken)
    {
        var mediaId = Guid.NewGuid();
        var bucketName = config["Storage:BucketName"]!;

        // Tạo Asset trạng thái Pending
        var mediaAsset = MediaAsset.Create(
            mediaId,
            MovieId.Of(command.RefId),
            command.FileName,
            command.ContentType,
            bucketName,
            command.Type
        );

        dbContext.MediaAssets.Add(mediaAsset);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Lấy Presigned URL để Client upload
        var uploadUrl = await storageService.GetPresignedUrlAsync(
            mediaAsset.Storage, 
            mediaAsset.Metadata.ContentType, 
            TimeSpan.FromMinutes(60)
        );

        return new CreateMediaUploadResult(mediaAsset.Id, uploadUrl);
    }
}