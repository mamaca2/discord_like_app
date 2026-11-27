using System.Security.Claims;
using DiscordApp.Api.Hubs;
using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DiscordApp.Api.Controllers;

[Route("api/server-invite")]
[ApiController]
[Authorize]
public class ServerInviteController(
    IServerInviteService serverInviteService,
    IHubContext<DirectMessageHub> directMessageHub,
    IHubContext<ServerHub> serverHub
) : BaseApiController
{
    [HttpPost("{serverId}/send/{friendId}")]
    public async Task<ActionResult<DirectMessageResponseDto>> CreateInviteAndSendAsync(
        int serverId,
        string friendId,
        [FromBody] CreateInviteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverInviteService.CreateInviteAndSendAsync(
            serverId, userId, friendId, dto);

        if (!result.IsSuccess)
            return ToActionResult(result);

        await directMessageHub.Clients.Users(new[] { userId, friendId })
            .SendAsync("ReceiveMessage", result.Value);

        return ToActionResult(result);
    }

    [HttpPost("join/{code}")]
    public async Task<ActionResult<ServerDto>> JoinServer(string code)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverInviteService.AcceptInviteAsync(code, userId);

        if (!result.IsSuccess)
            return ToActionResult(result);

        var server = result.Value!;

        await serverHub.Clients.Group($"server-{server.Id}")
            .SendAsync("ServerMemberJoined", new
            {
                ServerId = server.Id,
                UserId = userId
            });

        return ToActionResult(result);
    }
}
