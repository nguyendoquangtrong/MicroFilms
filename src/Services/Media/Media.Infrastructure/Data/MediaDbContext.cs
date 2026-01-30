using Media.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Media.Application.Data;

namespace Media.Infrastructure.Data;

public class MediaDbContext : DbContext , IApplicationDbontext
{
    public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options) { }

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}