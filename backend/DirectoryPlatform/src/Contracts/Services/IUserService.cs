using DirectoryPlatform.Contracts.DTOs.User;

namespace DirectoryPlatform.Contracts.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
    Task DeleteAsync(Guid id);
    Task<UserDto> UpdateRoleAsync(Guid id, string role);
    Task<UserDto> ToggleLockAsync(Guid id);
}
