using System.Security.Claims;
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.Api.Controllers;

[ApiController]

[Route("api/servers")]
[Authorize]
public class ServersController(
    IServerService serverService,
    IServerInviteService serverInviteService
) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ServerDto>> CreateServer([FromForm] CreateServerDto createServerDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverService.CreateServerAsync(userId, createServerDto);
        return ToActionResult(result);
    }

    [HttpGet("user-servers")]
    public async Task<ActionResult<List<ServerDto>>> GetUserServers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverService.GetUserServersAsync(userId);
        return ToActionResult(result);
    }

    [HttpPost("{serverId}/create-chat")]
    public async Task<ActionResult<ServerChatDto>> CreateServerChat(
        int serverId,
        [FromBody] CreateChatDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return ToActionResult(
            await serverService.CreateChatAsync(serverId, userId, dto)
        );
    }

    [HttpGet("{serverId}/get-chats")]
    public async Task<ActionResult<List<ServerChatDto>>> GetServerChatsAsync(
        int serverId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverService.GetChatAsync(serverId, userId);
        return ToActionResult(result);
    }

    [HttpGet("{serverId}/chats/{chatId}/messages")]
    public async Task<ActionResult<List<ServerChatMessageDto>>> GetServerChatMessages(
        int serverId,
        int chatId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        return ToActionResult(
            await serverService.GetServerChatMessagesAsync(
                chatId, serverId, userId, page, pageSize));
    }
}