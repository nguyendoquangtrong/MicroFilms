using BuildingBlocks.CQRS;
using Catalog.API.Domain.Entities;
using FluentValidation;
using Marten;

namespace Catalog.API.Features.Movies.CreateMovie;

public record CreateMovieCommand(
    string Title,
    string Description,
    DateTime ReleaseDate
    ) : ICommand<CreateMovieResult>;
    
public record CreateMovieResult(Guid Id);

public class CreateMovieValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class CreateMovieHandler(IDocumentSession session) : ICommandHandler<CreateMovieCommand, CreateMovieResult>
{
    public async Task<CreateMovieResult> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
    {
        // object
        var movie = new Movie
        {
            Title = command.Title,
            Description = command.Description,
            ReleaseDate = command.ReleaseDate,
            Status = VideoStatus.Pending
        };
        session.Store(movie);
        await session.SaveChangesAsync(cancellationToken);
        return new CreateMovieResult(movie.Id);
    }
}