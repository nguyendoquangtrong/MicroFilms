using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Marten;
using MassTransit;
using Media.Application.Abstractions;
using Media.Domain.Models;

namespace Media.Application.Features.ConfirmUpload;

public record ConfirmUploadCommand(Guid MediaId) : ICommand<ConfirmUploadResult>;
public record ConfirmUploadResult(bool IsSuccess);

public class ConfirmUploadHandler(
    IDocumentSession session,
    IStorageService storageService,
    IPublishEndpoint publishEndpoint
) : ICommandHandler<ConfirmUploadCommand, ConfirmUploadResult>
{
    public async Task<ConfirmUploadResult> Handle(ConfirmUploadCommand command, CancellationToken cancellationToken)
    {
        var mediaAsset = await session.LoadAsync<MediaAsset>(command.MediaId, cancellationToken);
        if (mediaAsset is null) throw new NotFoundException("MediaAsset", command.MediaId);

        // Kiểm tra file thật trên S3
        var metadata = await storageService.GetFileMetadataAsync(mediaAsset.Storage);
        if (metadata is null)
        {
            mediaAsset.MarkAsFailed("File not found on storage");
            session.Update(mediaAsset);
            await session.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("Upload verification failed: File not found.");
        }

        // Confirm & Publish Event
        mediaAsset.ConfirmUpload(metadata.SizeBytes);
        session.Update(mediaAsset);
        
        // Publish Integration Event (nếu cần map sang BuildingBlocks) hoặc Domain Event
        // Ở đây Marten sẽ tự publish Domain Event nếu cấu hình, hoặc ta publish thủ công:
        foreach (var evt in mediaAsset.ClearDomainEvents())
        {
            await publishEndpoint.Publish(evt, cancellationToken);
        }

        await session.SaveChangesAsync(cancellationToken);
        return new ConfirmUploadResult(true);
    }
}