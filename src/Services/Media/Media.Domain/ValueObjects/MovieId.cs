using Media.Domain.Exceptions;

namespace Media.Domain.ValueObjects;

public record MovieId
{
    public Guid Value { get; }
    private MovieId (Guid value) =>  Value = value;

    public static MovieId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
            throw new DomainException("ProductId cannot be empty");
        return new MovieId(value);
    }
}