using ChatApp.Api.DTOs.Users;

namespace ChatApp.Api.Services.Interfaces;

public interface IUserService
{
	Task<IReadOnlyList<UserDto>> GetAllExceptAsync(int currentUserId);
	Task<UserDto?> GetByIdAsync(int userId);
}
