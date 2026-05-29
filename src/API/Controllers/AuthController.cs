using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Authenticate user", Description = "Login with email and password to receive JWT token")]
    [SwaggerResponse(200, "Authentication successful", typeof(AuthResponseDto))]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user", Description = "Create a new user account")]
    [SwaggerResponse(201, "User created successfully", typeof(AuthResponseDto))]
    [SwaggerResponse(400, "Invalid input data")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        var result = await _authService.RegisterAsync(createUserDto);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.User.Id }, result);
    }

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Get current user", Description = "Retrieve current authenticated user information")]
    [SwaggerResponse(200, "User information", typeof(UserDto))]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guid))
        {
            return Unauthorized();
        }

        var user = await _authService.GetCurrentUserAsync(guid);
        return Ok(user);
    }
}
