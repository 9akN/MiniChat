using ChatApp.Api.DTOs.Auth;
using ChatApp.Api.DTOs.Users;
using ChatApp.Api.Models;
using ChatApp.Api.Repositories.Interfaces;
using ChatApp.Api.Services.Interfaces;

namespace ChatApp.Api.Services;

public class AuthService(IUnitOfWork unitOfWork, ITokenService tokenService) : IAuthService
{
	public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
	{
		var existing = await unitOfWork.Users.FindAsync(u => u.Username == request.Username);
		if (existing.Count > 0)
			throw new InvalidOperationException("Username is already taken.");

		var user = new User
		{
			Username = request.Username,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
			DisplayName = request.DisplayName,
			CreatedAt = DateTime.UtcNow,
		};

		await unitOfWork.Users.AddAsync(user);
		await unitOfWork.SaveChangesAsync();

		return BuildAuthResponse(user);
	}

	public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
	{
		var matches = await unitOfWork.Users.FindAsync(u => u.Username == request.Username);
		var user = matches.FirstOrDefault();

		if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
			throw new UnauthorizedAccessException("Invalid username or password.");

		return BuildAuthResponse(user);
	}

	private AuthResponseDto BuildAuthResponse(User user)
	{
		var (token, expiresAt) = tokenService.GenerateToken(user);
		return new AuthResponseDto
		{
			Token = token,
			ExpiresAt = expiresAt,
			User = new UserDto
			{
				Id = user.Id,
				Username = user.Username,
				DisplayName = user.DisplayName,
				AvatarUrl = user.AvatarUrl,
			},
		};
	}
}
