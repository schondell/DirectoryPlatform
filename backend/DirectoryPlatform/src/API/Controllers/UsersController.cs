using DirectoryPlatform.Contracts.DTOs.User;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid id, [FromBody] string role)
    {
        try
        {
            var user = await _userService.UpdateRoleAsync(id, role);
            return Ok(user);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{id}/toggle-lock")]
    public async Task<ActionResult<UserDto>> ToggleLock(Guid id)
    {
        try
        {
            var user = await _userService.ToggleLockAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
