using System.Security.Claims;
using DiscordApp.Application.DTOs.ServerDTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ServerChatHub(IAppDbContext dbContext) : Hub
{
    private static string ChatGroup(int chatId) => $"chat-{chatId}";

    private async Task<string> CheckAccess(int serverId, int chatId)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new HubException("Unauthorized.");

        var isMember = await dbContext.ServerMembers
            .AnyAsync(m => m.ServerInfoId == serverId && m.UserId == userId);

        var chatExists = await dbContext.ServerChats
            .AnyAsync(c => c.Id == chatId && c.ServerId == serverId);

        if (!isMember || !chatExists)
            throw new HubException("You cannot access this chat.");

        return userId;
    }

    public async Task JoinChat(int serverId, int chatId)
    {
        await CheckAccess(serverId, chatId);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            ChatGroup(chatId)
        );
    }

    public async Task SendChatMessage(
        int serverId,
        int chatId,
        string content)
    {
        var userId = await CheckAccess(serverId, chatId);

        if (string.IsNullOrWhiteSpace(content))
            throw new HubException("Message cannot be empty.");

        var message = new ServerChatMessage
        {
            ServerChatId = chatId,
            SenderId = userId,
            Content = content.Trim()
        };

        dbContext.ServerChatMessages.Add(message);
        await dbContext.SaveChangesAsync();

        var dto = new ServerChatMessageDto
        {
            Id = message.Id,
            ChatId = chatId,
            SenderId = userId,
            Content = message.Content,
            IsMine = true
        };

        await Clients.Caller.SendAsync("ReceiveChatMessage", dto);

        dto.IsMine = false;

        await Clients.OthersInGroup(ChatGroup(chatId))
            .SendAsync("ReceiveChatMessage", dto);
    }
}
