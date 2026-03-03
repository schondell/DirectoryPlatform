using DirectoryPlatform.Contracts.DTOs.AiListing;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
[Route("api/ailisting")]
public class AiListingController : BaseController
{
    private readonly IAiListingService _aiListingService;

    public AiListingController(IAiListingService aiListingService)
    {
        _aiListingService = aiListingService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<AiGeneratedListingDto>> Generate([FromBody] AiGenerateListingRequestDto request)
    {
        var result = await _aiListingService.GenerateListingAsync(request);
        return Ok(result);
    }

    [HttpPost("improve")]
    public async Task<ActionResult<AiGeneratedListingDto>> Improve([FromBody] AiImproveListingRequestDto request)
    {
        var result = await _aiListingService.ImproveListingAsync(request);
        return Ok(result);
    }
}
