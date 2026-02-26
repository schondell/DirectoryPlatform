namespace DirectoryPlatform.Core.Entities.Bookkeeping;

public class InvoiceLineItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int DisplayOrder { get; set; }
}
