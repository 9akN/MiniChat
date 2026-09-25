using System.Security.Claims;
using ChatApp.Api.DTOs.Users;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
	private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll() =>
		Ok(await userService.GetAllExceptAsync(CurrentUserId));

	[HttpGet("{id:int}")]
	public async Task<ActionResult<UserDto>> GetById(int id)
	{
		var user = await userService.GetByIdAsync(id);
		return user is null ? NotFound() : Ok(user);
	}
}
