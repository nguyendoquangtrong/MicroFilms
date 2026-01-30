using Media.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Media.Application.Data;

public interface IApplicationDbontext
{
    DbSet<MediaAsset>  MediaAssets { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}