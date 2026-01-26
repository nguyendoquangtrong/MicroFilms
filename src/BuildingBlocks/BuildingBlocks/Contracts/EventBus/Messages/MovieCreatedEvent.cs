namespace BuildingBlocks.Contracts.EventBus.Messages;

public record MovieCreatedEvent(Guid Id, string Title, string Description);