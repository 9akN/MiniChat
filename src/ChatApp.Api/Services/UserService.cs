using ChatApp.Api.DTOs.Users;
using ChatApp.Api.Repositories.Interfaces;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
	public async Task<IReadOnlyList<UserDto>> GetAllExceptAsync(int currentUserId) =>
		await unitOfWork.Users.Query()
			.Where(u => u.Id != currentUserId)
			.OrderBy(u => u.DisplayName)
			.Select(u => new UserDto { Id = u.Id, Username = u.Username, DisplayName = u.DisplayName, AvatarUrl = u.AvatarUrl })
			.ToListAsync();

	public async Task<UserDto?> GetByIdAsync(int userId)
	{
		var user = await unitOfWork.Users.GetByIdAsync(userId);
		return user is null
			? null
			: new UserDto { Id = user.Id, Username = user.Username, DisplayName = user.DisplayName, AvatarUrl = user.AvatarUrl };
	}
}
