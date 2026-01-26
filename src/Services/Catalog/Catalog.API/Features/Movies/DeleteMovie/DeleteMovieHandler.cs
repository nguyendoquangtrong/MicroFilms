namespace Catalog.API.Features.Movies.DeleteMovie;

public record DeleteMovieCommand(Guid Id) : ICommand<DeleteMovieResult>;
public record DeleteMovieResult(bool IsSuccess);

public class DeleteCommandValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}

public class DeleteCommandHandler(IDocumentSession session) : ICommandHandler<DeleteMovieCommand, DeleteMovieResult>
{
    public async Task<DeleteMovieResult> Handle(DeleteMovieCommand command, CancellationToken cancellationToken)
    {
        session.Delete<Movie>(command.Id);
        await session.SaveChangesAsync(cancellationToken);
        return new DeleteMovieResult(true);
    }
} 