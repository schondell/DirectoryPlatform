namespace DirectoryPlatform.Contracts.DTOs.Message;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string? SenderName { get; set; }
    public Guid RecipientId { get; set; }
    public string? RecipientName { get; set; }
    public Guid? ListingId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
