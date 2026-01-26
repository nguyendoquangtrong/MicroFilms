using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions;

public class MovieNotFoundException : NotFoundException
{
    public MovieNotFoundException(Guid Id) : base("Product", Id)
    {
    }
}