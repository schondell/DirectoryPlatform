namespace DirectoryPlatform.Contracts.DTOs.AiListing;

public class AiGenerateListingRequestDto
{
    public Guid CategoryId { get; set; }
    public string UserInput { get; set; } = string.Empty;
    public string Language { get; set; } = "fr";
}

public class AiImproveListingRequestDto
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "fr";
}

public class AiGeneratedListingDto
{
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? SuggestedPriceMin { get; set; }
    public decimal? SuggestedPriceMax { get; set; }
    public List<AiSuggestedAttributeDto> Attributes { get; set; } = [];
}

public class AiSuggestedAttributeDto
{
    public Guid AttributeDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
