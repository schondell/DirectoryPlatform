using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Entities.Bookkeeping;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingDetail> ListingDetails => Set<ListingDetail>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<ListingAttribute> ListingAttributes => Set<ListingAttribute>();
    public DbSet<ListingMedia> ListingMedia => Set<ListingMedia>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<ListingLanguage> ListingLanguages => Set<ListingLanguage>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionTier> SubscriptionTiers => Set<SubscriptionTier>();
    public DbSet<SubscriptionFeature> SubscriptionFeatures => Set<SubscriptionFeature>();
    public DbSet<PricingTier> PricingTiers => Set<PricingTier>();
    public DbSet<CouponCode> CouponCodes => Set<CouponCode>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ListingApprovalHistory> ListingApprovalHistories => Set<ListingApprovalHistory>();

    // Engagement
    public DbSet<ListingLike> ListingLikes => Set<ListingLike>();
    public DbSet<ListingFollower> ListingFollowers => Set<ListingFollower>();
    public DbSet<ListingPageView> ListingPageViews => Set<ListingPageView>();
    public DbSet<ListingVisitor> ListingVisitors => Set<ListingVisitor>();
    public DbSet<VisitorMetric> VisitorMetrics => Set<VisitorMetric>();
    public DbSet<ListingBoost> ListingBoosts => Set<ListingBoost>();

    // Financial
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentReconciliation> PaymentReconciliations => Set<PaymentReconciliation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.RefreshToken).IsUnique().HasFilter("refresh_token IS NOT NULL");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.PasswordSalt).HasMaxLength(512);
            entity.HasOne(e => e.OwnerUser).WithMany().HasForeignKey(e => e.OwnerUserId).OnDelete(DeleteBehavior.SetNull);
        });

        // Listing
        modelBuilder.Entity<Listing>(entity =>
        {
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.RegionId);
            entity.HasIndex(e => new { e.Status, e.Weight });
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.HasOne(e => e.Category).WithMany(c => c.Listings).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Region).WithMany(r => r.Listings).HasForeignKey(e => e.RegionId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.User).WithMany(u => u.Listings).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingDetail 1:1
        modelBuilder.Entity<ListingDetail>(entity =>
        {
            entity.HasIndex(e => e.ListingId).IsUnique();
            entity.HasOne(e => e.Listing).WithOne(l => l.Detail).HasForeignKey<ListingDetail>(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.AvailabilityHours).HasColumnType("jsonb");
            entity.Property(e => e.PriceInfo).HasColumnType("jsonb");
            entity.Property(e => e.PaymentMethods).HasColumnType("jsonb");
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.HasOne(e => e.Parent).WithMany(c => c.Children).HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Region
        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.HasOne(e => e.Parent).WithMany(r => r.Children).HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        // AttributeDefinition
        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasIndex(e => new { e.CategoryId, e.Slug }).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Options).HasColumnType("jsonb");
            entity.HasOne(e => e.Category).WithMany(c => c.AttributeDefinitions).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingAttribute — composite index for filtering
        modelBuilder.Entity<ListingAttribute>(entity =>
        {
            entity.HasIndex(e => new { e.AttributeDefinitionId, e.Value });
            entity.HasIndex(e => e.ListingId);
            entity.Property(e => e.Value).HasMaxLength(1000);
            entity.HasOne(e => e.Listing).WithMany(l => l.Attributes).HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.AttributeDefinition).WithMany(a => a.ListingAttributes).HasForeignKey(e => e.AttributeDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingMedia
        modelBuilder.Entity<ListingMedia>(entity =>
        {
            entity.HasIndex(e => e.ListingId);
            entity.Property(e => e.Url).HasMaxLength(1000);
            entity.HasOne(e => e.Listing).WithMany(l => l.Media).HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(e => e.ListingId);
            entity.HasOne(e => e.Listing).WithMany(l => l.Reviews).HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.Reviews).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Language
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NativeName).HasMaxLength(100);
        });

        // ListingLanguage
        modelBuilder.Entity<ListingLanguage>(entity =>
        {
            entity.HasIndex(e => new { e.ListingId, e.LanguageId }).IsUnique();
            entity.HasOne(e => e.Listing).WithMany(l => l.Languages).HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Language).WithMany(l => l.ListingLanguages).HasForeignKey(e => e.LanguageId).OnDelete(DeleteBehavior.Cascade);
        });

        // Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.SenderId);
            entity.Property(e => e.Subject).HasMaxLength(256);
            entity.HasOne(e => e.Sender).WithMany(u => u.SentMessages).HasForeignKey(e => e.SenderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Recipient).WithMany(u => u.ReceivedMessages).HasForeignKey(e => e.RecipientId).OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.IsRead });
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.IsActive });
            entity.HasOne(e => e.User).WithMany(u => u.Subscriptions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.SubscriptionTier).WithMany(t => t.Subscriptions).HasForeignKey(e => e.SubscriptionTierId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CouponCode).WithMany().HasForeignKey(e => e.CouponCodeId).OnDelete(DeleteBehavior.SetNull);
        });

        // SubscriptionTier
        modelBuilder.Entity<SubscriptionTier>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualPrice).HasPrecision(10, 2);
        });

        // SubscriptionFeature
        modelBuilder.Entity<SubscriptionFeature>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.HasOne(e => e.SubscriptionTier).WithMany(t => t.Features).HasForeignKey(e => e.SubscriptionTierId).OnDelete(DeleteBehavior.Cascade);
        });

        // PricingTier
        modelBuilder.Entity<PricingTier>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // CouponCode
        modelBuilder.Entity<CouponCode>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.MaxDiscountAmount).HasPrecision(10, 2);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.OldValues).HasColumnType("jsonb");
            entity.Property(e => e.NewValues).HasColumnType("jsonb");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        // ListingApprovalHistory
        modelBuilder.Entity<ListingApprovalHistory>(entity =>
        {
            entity.HasIndex(e => e.ListingId);
            entity.HasOne(e => e.Listing).WithMany(l => l.ApprovalHistory).HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ReviewedByUser).WithMany().HasForeignKey(e => e.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        // ListingLike
        modelBuilder.Entity<ListingLike>(entity =>
        {
            entity.HasIndex(e => new { e.ListingId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingFollower
        modelBuilder.Entity<ListingFollower>(entity =>
        {
            entity.HasIndex(e => new { e.ListingId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingPageView — composite key
        modelBuilder.Entity<ListingPageView>(entity =>
        {
            entity.HasKey(e => new { e.ListingId, e.ViewDate });
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
        });

        // ListingVisitor
        modelBuilder.Entity<ListingVisitor>(entity =>
        {
            entity.HasIndex(e => new { e.ListingId, e.UserId });
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // VisitorMetric
        modelBuilder.Entity<VisitorMetric>(entity =>
        {
            entity.HasIndex(e => e.Date).IsUnique();
        });

        // ListingBoost
        modelBuilder.Entity<ListingBoost>(entity =>
        {
            entity.HasIndex(e => new { e.ListingId, e.StartsAt, e.ExpiresAt });
            entity.Property(e => e.AmountPaid).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(e => e.IsActive);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasPrecision(10, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(10, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Listing).WithMany().HasForeignKey(e => e.ListingId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Subscription).WithMany().HasForeignKey(e => e.SubscriptionId).OnDelete(DeleteBehavior.SetNull);
        });

        // InvoiceLineItem
        modelBuilder.Entity<InvoiceLineItem>(entity =>
        {
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
            entity.HasOne(e => e.Invoice).WithMany(i => i.LineItems).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.InvoiceId);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.TransactionReference).HasMaxLength(256);
            entity.HasOne(e => e.Invoice).WithMany(i => i.Payments).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        // PaymentReconciliation
        modelBuilder.Entity<PaymentReconciliation>(entity =>
        {
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.HasOne(e => e.Payment).WithMany().HasForeignKey(e => e.PaymentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RecordedByUser).WithMany().HasForeignKey(e => e.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
