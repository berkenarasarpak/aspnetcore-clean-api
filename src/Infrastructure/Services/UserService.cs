using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        var query = await _unitOfWork.Repository<User>().GetAllAsync();
        var users = query.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            users = users.Where(u => 
                u.Email.ToLower().Contains(term) || 
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term));
        }

        var totalCount = users.Count();
        var items = users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<UserDto>
        {
            Items = items.Select(MapToUserDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.IsActive = dto.IsActive;

        await _unitOfWork.Repository<User>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToUserDto(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        await _unitOfWork.Repository<User>().DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> UserExistsAsync(Guid id)
    {
        return await _unitOfWork.Repository<User>().ExistsAsync(u => u.Id == id);
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
