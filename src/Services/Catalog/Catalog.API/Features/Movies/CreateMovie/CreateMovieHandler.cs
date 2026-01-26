namespace Catalog.API.Features.Movies.CreateMovie;

// Giữ nguyên Command và Result
public record CreateMovieCommand(
    string Title,
    string Description,
    DateTime ReleaseDate,
    List<string> Genres // Thêm Genres cho đầy đủ
    ) : ICommand<CreateMovieResult>;

public record CreateMovieResult(Guid Id);

// Validator (Đã có, giữ nguyên hoặc bổ sung)
public class CreateMovieValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(2, 150);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
    }
}

// Handler nâng cấp
public class CreateMovieHandler(
    IDocumentSession session, 
    IPublishEndpoint publishEndpoint, // Inject MassTransit
    ILogger<CreateMovieHandler> logger
    ) : ICommandHandler<CreateMovieCommand, CreateMovieResult>
{
    public async Task<CreateMovieResult> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
    {
        // 1. Tạo Entity
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            ReleaseDate = command.ReleaseDate,
            Genres = command.Genres,
            Status = VideoStatus.Pending, // Pending chờ upload video
            CreatedAt = DateTime.UtcNow
        };

        // 2. Lưu vào Database (Marten)
        session.Store(movie);

        // 3. Bắn Event (Async)
        // AI Service sẽ lắng nghe event này để chuẩn bị Resource
        var eventMessage = new MovieCreatedEvent(movie.Id, movie.Title, movie.Description);
        await publishEndpoint.Publish(eventMessage, cancellationToken);

        // 4. Commit Transaction
        await session.SaveChangesAsync(cancellationToken);

        return new CreateMovieResult(movie.Id);
    }
}