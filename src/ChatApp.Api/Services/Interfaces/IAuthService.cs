using ChatApp.Api.DTOs.Auth;

namespace ChatApp.Api.Services.Interfaces;

public interface IAuthService
{
	Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
	Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}
