using ChatApp.Api.DTOs.Auth;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
	{
		try
		{
			return Ok(await authService.RegisterAsync(request));
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(new { message = ex.Message });
		}
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
	{
		try
		{
			return Ok(await authService.LoginAsync(request));
		}
		catch (UnauthorizedAccessException ex)
		{
			return Unauthorized(new { message = ex.Message });
		}
	}
}
