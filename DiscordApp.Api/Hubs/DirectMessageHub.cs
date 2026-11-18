using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DiscordApp.Api.Hubs;

[Authorize]
public class DirectMessageHub(
    IDirectMessageService messageService
) : Hub
{
    public async Task SendMessage(DirectMessageDTO dto)
    {
        var senderId = Context.UserIdentifier;

        if (string.IsNullOrEmpty(senderId))
            throw new HubException("Unauthorized.");

        var result = await messageService.SendMessageAsync(senderId, dto);

        if (!result.IsSuccess)
            throw new HubException("Failed to send message.");

        await Clients.Users(new[] { senderId, dto.ReceiverId })
            .SendAsync("ReceiveMessage", result.Value);
    }
}