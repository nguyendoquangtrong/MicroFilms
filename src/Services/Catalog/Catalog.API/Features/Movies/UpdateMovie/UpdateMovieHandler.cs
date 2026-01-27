namespace Catalog.API.Features.Movies.UpdateMovie;

public record UpdateMovieCommand(
    Guid Id,
    string Title,
    string Description,
    string? PosterUrl,
    string? VideoUrl,
    DateTime ReleaseDate,
    List<string> Genres,
    VideoStatus Status
    ) : ICommand<UpdateMovieResult>;
public record UpdateMovieResult(bool IsSuccess);

public class UpdateMovieCommandValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .Length(2,150).WithMessage("Title must be between 2 and 150 characters");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}

internal class UpdateProductCommandHandler(IDocumentSession session, IPublisher publisher) : ICommandHandler<UpdateMovieCommand,UpdateMovieResult>
{
    public async Task<UpdateMovieResult> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await session.LoadAsync<Movie>(command.Id, cancellationToken);
        if (movie is null)
            throw new MovieNotFoundException(command.Id);
        movie.Title = command.Title;
        movie.Description = command.Description;
        movie.PosterUrl = command.PosterUrl;
        movie.VideoUrl = command.VideoUrl;
        movie.ReleaseDate = command.ReleaseDate;
        movie.Genres = command.Genres;
        movie.Status = command.Status;
        movie.UpdatedAt = DateTime.UtcNow;
        
        session.Update(movie);
        await session.SaveChangesAsync(cancellationToken);
        await publisher.Publish(new MovieUpdatedEvent(command.Id), cancellationToken);
        return new UpdateMovieResult(true);
    }
}