using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.DTOs;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;
using LoginRequest = TaskTracker.API.DTOs.LoginRequest;
using RegisterRequest = TaskTracker.API.DTOs.RegisterRequest;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IUserRepository userRepository, ILogger<AuthController> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required");

            // Check if user already exists
            if (await _userRepository.UserExistsAsync(request.Username, request.Email))
                return BadRequest("Username or email already exists");

            // Create user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Generate token
            var token = _authService.GenerateJwtToken(createdUser);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = createdUser.Id,
                Username = createdUser.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return Problem("An error occurred during registration");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Problem("An error occurred during login");
        }
    }
}