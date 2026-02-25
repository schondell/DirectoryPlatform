using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; }
    public ICategoryRepository Categories { get; }
    public IRegionRepository Regions { get; }
    public IListingRepository Listings { get; }
    public IAttributeDefinitionRepository AttributeDefinitions { get; }
    public IListingAttributeRepository ListingAttributes { get; }
    public IListingDetailRepository ListingDetails { get; }
    public IListingMediaRepository ListingMedia { get; }
    public IReviewRepository Reviews { get; }
    public IMessageRepository Messages { get; }
    public INotificationRepository Notifications { get; }
    public ISubscriptionRepository Subscriptions { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Categories = new CategoryRepository(context);
        Regions = new RegionRepository(context);
        Listings = new ListingRepository(context);
        AttributeDefinitions = new AttributeDefinitionRepository(context);
        ListingAttributes = new ListingAttributeRepository(context);
        ListingDetails = new ListingDetailRepository(context);
        ListingMedia = new ListingMediaRepository(context);
        Reviews = new ReviewRepository(context);
        Messages = new MessageRepository(context);
        Notifications = new NotificationRepository(context);
        Subscriptions = new SubscriptionRepository(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
