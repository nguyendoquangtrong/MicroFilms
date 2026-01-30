using Media.Domain.Models;
using Media.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Media.Infrastructure.Data.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        // Tên bảng
        builder.ToTable("MediaAssets");

        // Khóa chính
        builder.HasKey(x => x.Id);

        // 1. Map MovieId (Value Object -> Guid Column)
        builder.Property(x => x.MovieId)
            .HasConversion(id => id.Value, value => MovieId.Of(value))
            .IsRequired();

        // 2. Map StorageLocation (Value Object -> Columns: Storage_Bucket, Storage_Key)
        builder.OwnsOne(x => x.Storage, storage =>
        {
            storage.Property(s => s.Bucket).HasColumnName("Storage_Bucket").IsRequired();
            storage.Property(s => s.Key).HasColumnName("Storage_Key").IsRequired();
        });

        // 3. Map FileMetadata (Value Object -> Columns: Metadata_FileName, Metadata_Size, ...)
        builder.OwnsOne(x => x.Metadata, meta =>
        {
            meta.Property(m => m.FileName).HasColumnName("Metadata_FileName");
            meta.Property(m => m.ContentType).HasColumnName("Metadata_ContentType");
            meta.Property(m => m.Extension).HasColumnName("Metadata_Extension");
            meta.Property(m => m.SizeBytes).HasColumnName("Metadata_SizeBytes");
        });

        // Các trường Enum
        builder.Property(x => x.Status).HasConversion<string>(); // Lưu chữ "Uploaded" thay vì số
        builder.Property(x => x.Type).HasConversion<string>();
    }
}