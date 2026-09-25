using System.Security.Claims;
using ChatApp.Api.DTOs.Conversations;
using ChatApp.Api.DTOs.Messages;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController(IConversationService conversationService, IMessageService messageService) : ControllerBase
{
	private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetMine() =>
		Ok(await conversationService.GetForUserAsync(CurrentUserId));

	[HttpPost]
	public async Task<ActionResult<ConversationDto>> Create(CreateConversationDto request)
	{
		try
		{
			return Ok(await conversationService.GetOrCreateAsync(CurrentUserId, request.OtherUserId));
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet("{id:int}/messages")]
	public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessages(int id)
	{
		if (!await conversationService.IsParticipantAsync(id, CurrentUserId))
			return Forbid();

		return Ok(await messageService.GetForConversationAsync(id));
	}

	[HttpPut("{id:int}/read")]
	public async Task<IActionResult> MarkAsRead(int id)
	{
		if (!await conversationService.IsParticipantAsync(id, CurrentUserId))
			return Forbid();

		await messageService.MarkAsReadAsync(id, CurrentUserId);
		return NoContent();
	}
}
