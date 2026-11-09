using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services.ChatServices;

public class DirectMessagesService(IAppDbContext dbContext)
    : IDirectMessageService
{
    public async Task<Result> SendMessage(
        string senderId,
        DirectMessageDTO directMessageDto)
    {
        var senderExists = await dbContext.Users
            .AnyAsync(u => u.Id == senderId);

        if (!senderExists)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Sender not found.")
            );
        }

        var receiverExists = await dbContext.Users
            .AnyAsync(u => u.Id == directMessageDto.ReceiverId);

        if (!receiverExists)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Receiver not found.")
            );
        }

        var message = new DirectMessage
        {
            SenderId = senderId,
            ReceiverId = directMessageDto.ReceiverId,
            Content = directMessageDto.Content,
            SentAt = DateTime.UtcNow
        };

        dbContext.DirectMessages.Add(message);

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }
}