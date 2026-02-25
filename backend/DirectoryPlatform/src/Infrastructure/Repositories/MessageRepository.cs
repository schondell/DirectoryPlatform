using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Message>> GetInboxAsync(Guid userId)
        => await _dbSet.Include(m => m.Sender).Where(m => m.RecipientId == userId && !m.IsDeletedByRecipient).OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<IEnumerable<Message>> GetSentAsync(Guid userId)
        => await _dbSet.Include(m => m.Recipient).Where(m => m.SenderId == userId && !m.IsDeletedBySender).OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _dbSet.CountAsync(m => m.RecipientId == userId && !m.IsRead && !m.IsDeletedByRecipient);
}
