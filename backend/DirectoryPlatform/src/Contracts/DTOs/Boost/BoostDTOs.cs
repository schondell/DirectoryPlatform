namespace DirectoryPlatform.Contracts.DTOs.Boost;

public class CreateBoostDto
{
    public Guid ListingId { get; set; }
    public string BoostType { get; set; } = string.Empty;
    public int DurationDays { get; set; } = 7;
}

public class BoostDto
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string BoostType { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public double Multiplier { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "CHF";
    public bool IsActive { get; set; }
}

public class BoostPricingDto
{
    public string BoostType { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public double Multiplier { get; set; }
    public string Description { get; set; } = string.Empty;
}
