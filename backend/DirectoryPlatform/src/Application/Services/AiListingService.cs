using System.Text.Json;
using DirectoryPlatform.Contracts.DTOs.AiListing;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DirectoryPlatform.Application.Services;

public class AiListingService : IAiListingService
{
    private readonly IClaudeService _claudeService;
    private readonly IAttributeDefinitionRepository _attributeRepo;
    private readonly IScraperAnalyticsRepository _scraperRepo;
    private readonly ILogger<AiListingService> _logger;

    private static readonly JsonSerializerOptions ParseOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiListingService(
        IClaudeService claudeService,
        IAttributeDefinitionRepository attributeRepo,
        IScraperAnalyticsRepository scraperRepo,
        ILogger<AiListingService> logger)
    {
        _claudeService = claudeService;
        _attributeRepo = attributeRepo;
        _scraperRepo = scraperRepo;
        _logger = logger;
    }

    public async Task<AiGeneratedListingDto> GenerateListingAsync(AiGenerateListingRequestDto request)
    {
        var systemPrompt = await BuildSystemPromptAsync(request.CategoryId, request.Language);
        var userMessage = $"Create a professional listing for: {request.UserInput}";
        var response = await _claudeService.SendMessageAsync(systemPrompt, userMessage);
        return ParseResponse(response);
    }

    public async Task<AiGeneratedListingDto> ImproveListingAsync(AiImproveListingRequestDto request)
    {
        var systemPrompt = await BuildSystemPromptAsync(request.CategoryId, request.Language);
        var userMessage = $"Improve this existing listing.\nCurrent title: {request.Title}\nCurrent description: {request.Description}\nMake it more professional, engaging, and complete. Keep the same meaning but improve the quality.";
        var response = await _claudeService.SendMessageAsync(systemPrompt, userMessage);
        return ParseResponse(response);
    }

    private async Task<string> BuildSystemPromptAsync(Guid categoryId, string language)
    {
        var attributes = await _attributeRepo.GetByCategoryIdAsync(categoryId);
        var priceData = await _scraperRepo.GetPriceDistributionAsync();

        var attributeSchemas = attributes.Select(a => new
        {
            attributeDefinitionId = a.Id.ToString(),
            name = a.Name,
            slug = a.Slug,
            type = a.Type.ToString(),
            options = string.IsNullOrEmpty(a.Options) ? null : a.Options,
            unit = a.Unit,
            minValue = a.MinValue,
            maxValue = a.MaxValue,
            isRequired = a.IsRequired
        });

        var attributeJson = JsonSerializer.Serialize(attributeSchemas);

        var priceContext = "";
        if (priceData.Count > 0)
        {
            var first = priceData[0];
            priceContext = $"Price context from market data: median={first.Median} CHF, range={first.Min}-{first.Max} CHF. ";
        }

        var languageNames = new Dictionary<string, string>
        {
            ["fr"] = "French",
            ["de"] = "German",
            ["it"] = "Italian",
            ["en"] = "English"
        };
        var langName = languageNames.GetValueOrDefault(language, "French");

        return $$"""
            You are a Swiss classified ads expert for petitesannonces.ch. Generate a professional listing in {{langName}}.

            Category attributes (use these exact attributeDefinitionId values in your response):
            {{attributeJson}}

            {{priceContext}}

            Return ONLY a JSON object with this exact structure (no markdown, no code fences, no explanation):
            {
              "title": "concise, attractive title",
              "shortDescription": "1-2 sentence summary",
              "description": "detailed description with key selling points, condition, features",
              "suggestedPriceMin": 0,
              "suggestedPriceMax": 0,
              "attributes": [
                {
                  "attributeDefinitionId": "exact-guid-from-above",
                  "name": "attribute name",
                  "value": "appropriate value matching the type and options"
                }
              ]
            }

            Rules:
            - For Select/MultiSelect attributes, use only values from the provided options
            - For Number attributes, respect minValue/maxValue constraints and include the unit
            - For Boolean attributes, use "true" or "false"
            - suggestedPriceMin/Max should be realistic CHF prices based on the market data provided
            - If no price data is available, suggest reasonable Swiss market prices
            - Write in {{langName}} only
            - Be professional and persuasive
            """;
    }

    private AiGeneratedListingDto ParseResponse(string response)
    {
        var cleaned = response.Trim();

        // Strip markdown code fences if Claude wrapped the JSON
        if (cleaned.StartsWith("```"))
        {
            var firstNewline = cleaned.IndexOf('\n');
            if (firstNewline >= 0)
                cleaned = cleaned[(firstNewline + 1)..];
            if (cleaned.EndsWith("```"))
                cleaned = cleaned[..^3];
            cleaned = cleaned.Trim();
        }

        try
        {
            var result = JsonSerializer.Deserialize<AiGeneratedListingDto>(cleaned, ParseOptions);
            return result ?? new AiGeneratedListingDto();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI response: {Response}", cleaned);
            return new AiGeneratedListingDto
            {
                Title = "Could not parse AI response",
                Description = cleaned
            };
        }
    }
}
