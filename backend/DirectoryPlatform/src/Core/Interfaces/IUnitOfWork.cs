namespace DirectoryPlatform.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    IRegionRepository Regions { get; }
    IListingRepository Listings { get; }
    IAttributeDefinitionRepository AttributeDefinitions { get; }
    IListingAttributeRepository ListingAttributes { get; }
    IListingDetailRepository ListingDetails { get; }
    IListingMediaRepository ListingMedia { get; }
    IReviewRepository Reviews { get; }
    IMessageRepository Messages { get; }
    INotificationRepository Notifications { get; }
    ISubscriptionRepository Subscriptions { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
