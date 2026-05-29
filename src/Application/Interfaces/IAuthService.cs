using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(CreateUserDto createUserDto);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
    string GenerateJwtToken(Guid userId, string email, string role);
}
