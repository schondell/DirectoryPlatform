using DirectoryPlatform.Core.Entities.Scraper;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Data;

public class ScraperDbContext : DbContext
{
    public ScraperDbContext(DbContextOptions<ScraperDbContext> options) : base(options) { }

    public DbSet<ScraperCategory> Categories => Set<ScraperCategory>();
    public DbSet<ScraperListing> Listings => Set<ScraperListing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScraperCategory>(entity =>
        {
            entity.ToTable("petitesannonces_categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.ParentExternalId).HasColumnName("parent_external_id");
            entity.Property(e => e.ListingCount).HasColumnName("listing_count");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<ScraperListing>(entity =>
        {
            entity.ToTable("petitesannonces_listings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExternalId).HasColumnName("external_id");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.CategoryExternalId).HasColumnName("category_external_id");
            entity.Property(e => e.PhoneRaw).HasColumnName("phone_raw");
            entity.Property(e => e.PhoneNormalized).HasColumnName("phone_normalized");
            entity.Property(e => e.ImageHash).HasColumnName("image_hash");
            entity.Property(e => e.IsPaid).HasColumnName("is_paid");
            entity.Property(e => e.PaidType).HasColumnName("paid_type");
            entity.Property(e => e.ParsedData).HasColumnName("parsed_data").HasColumnType("jsonb");
            entity.Property(e => e.ImagesDownloaded).HasColumnName("images_downloaded");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.FirstSeenAt).HasColumnName("first_seen_at");
            entity.Property(e => e.LastSeenAt).HasColumnName("last_seen_at");
            entity.Property(e => e.ExpiredAt).HasColumnName("expired_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }
}
