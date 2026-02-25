namespace DirectoryPlatform.Core.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;
    public Guid RecipientId { get; set; }
    public User Recipient { get; set; } = null!;
    public Guid? ListingId { get; set; }
    public Listing? Listing { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsDeletedBySender { get; set; }
    public bool IsDeletedByRecipient { get; set; }
}
