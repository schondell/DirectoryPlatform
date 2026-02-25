using DirectoryPlatform.Contracts.DTOs.User;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
public class MeController : BaseController
{
    private readonly IUserService _userService;

    public MeController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var user = await _userService.GetByIdAsync(GetCurrentUserId());
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        try
        {
            var user = await _userService.UpdateAsync(GetCurrentUserId(), dto);
            return Ok(user);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
