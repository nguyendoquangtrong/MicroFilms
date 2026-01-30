using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions; 
using MassTransit;
using Media.Application.Abstractions;
using Media.Application.Data;
using Media.Domain.Models;


namespace Media.Application.Features.ConfirmUpload;

public record ConfirmUploadCommand(Guid MediaId) : ICommand<ConfirmUploadResult>;
public record ConfirmUploadResult(bool IsSuccess);

public class ConfirmUploadHandler(
    IApplicationDbontext dbContext,
    IStorageService storageService,
    IPublishEndpoint publishEndpoint
) : ICommandHandler<ConfirmUploadCommand, ConfirmUploadResult>
{
    public async Task<ConfirmUploadResult> Handle(ConfirmUploadCommand command, CancellationToken cancellationToken)
    {
        var mediaAsset = await dbContext.MediaAssets.FindAsync([command.MediaId], cancellationToken);        if (mediaAsset is null) throw new NotFoundException("MediaAsset", command.MediaId);

        // Kiểm tra file thật trên S3
        var metadata = await storageService.GetFileMetadataAsync(mediaAsset.Storage);
        if (metadata is null)
        {
            mediaAsset.MarkAsFailed("File not found on storage");
            dbContext.MediaAssets.Update(mediaAsset);
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("Upload verification failed: File not found.");
        }

        // Confirm & Publish Event
        mediaAsset.ConfirmUpload(metadata.SizeBytes);
        dbContext.MediaAssets.Update(mediaAsset);
        
        foreach (var evt in mediaAsset.ClearDomainEvents())
        {
            await publishEndpoint.Publish(evt, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ConfirmUploadResult(true);
    }
}