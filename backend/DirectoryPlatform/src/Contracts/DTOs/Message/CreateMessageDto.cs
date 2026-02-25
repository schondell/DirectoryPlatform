namespace DirectoryPlatform.Contracts.DTOs.Message;

public class CreateMessageDto
{
    public Guid RecipientId { get; set; }
    public Guid? ListingId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
