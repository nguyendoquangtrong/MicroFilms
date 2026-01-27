namespace Catalog.API.Infrastructure.Messaging;

public record MovieDeletedEvent(Guid Id) : INotification;
