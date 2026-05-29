using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users", Description = "Retrieve paginated list of users (Admin only)")]
    [SwaggerResponse(200, "List of users", typeof(PagedResult<UserDto>))]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _userService.GetAllUsersAsync(pageNumber, pageSize, search);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieve a specific user by ID (Admin only)")]
    [SwaggerResponse(200, "User found", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {id} not found" });
        }
        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update user", Description = "Update an existing user (Admin only)")]
    [SwaggerResponse(200, "User updated", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateUserAsync(id, dto);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete user", Description = "Delete a user (Admin only)")]
    [SwaggerResponse(204, "User deleted")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
