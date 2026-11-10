using System.Security.Claims;
using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.Api.Controllers;

[ApiController]
[Route("api/direct-messages")]
[Authorize]
public class DirectMessageController(
    IDirectMessageService messageService
) : BaseApiController
{
    [HttpPost("send")]
    public async Task<ActionResult> SendMessage(
        DirectMessageDTO directMessageDTO)
    {
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(senderId))
            return Unauthorized();

        var result = await messageService.SendMessageAsync(
            senderId,
            directMessageDTO
        );

        return ToActionResult(result);
    }

    [HttpGet("get-conversation/{friendId}")]
    public async Task<ActionResult<List<DirectMessageResponseDto>>> GetConversation(
        string friendId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await messageService.GetConversationAsync(
            userId,
            friendId
        );

        return ToActionResult(result);
    }
}