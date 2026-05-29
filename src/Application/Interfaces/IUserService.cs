using Application.DTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task DeleteUserAsync(Guid id);
    Task<bool> UserExistsAsync(Guid id);
}
