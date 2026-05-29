using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Repository<User>()
            .FindAsync(u => u.Email == loginDto.Email && u.IsActive);

        var userEntity = user.FirstOrDefault();
        if (userEntity == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, userEntity.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        userEntity.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Repository<User>().UpdateAsync(userEntity);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(userEntity.Id, userEntity.Email, userEntity.Role);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(userEntity)
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(CreateUserDto createUserDto)
    {
        var exists = await _unitOfWork.Repository<User>()
            .ExistsAsync(u => u.Email == createUserDto.Email);

        if (exists)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Role = createUserDto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(user.Id, user.Email, user.Role);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return MapToUserDto(user);
    }

    public string GenerateJwtToken(Guid userId, string email, string role)
    {
        return _jwtService.GenerateToken(userId, email, role);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
