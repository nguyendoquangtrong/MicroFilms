namespace Catalog.API.Infrastructure.Messaging;

public record MovieUpdatedEvent(Guid Id) : INotification;