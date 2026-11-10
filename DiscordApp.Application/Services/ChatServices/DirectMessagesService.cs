using DiscordApp.Application.DTOs.ChatDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services.ChatServices;

public class DirectMessagesService(
    IAppDbContext dbContext, 
    UserManager<User> userManager
    ) : IDirectMessageService
{
    public async Task<Result> SendMessageAsync(
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
    
    public async Task<Result<List<DirectMessageResponseDto>>> GetConversationAsync(
        string senderId,
        string receiverId)
    {
        var senderUser = await userManager.FindByIdAsync(senderId);

        if (senderUser is null)
        {
            return Result<List<DirectMessageResponseDto>>.NotFound(
                new Error(ErrorCodes.NotFound, "Sender not found.")
            );
        }

        var receiverUser = await userManager.FindByIdAsync(receiverId);

        if (receiverUser is null)
        {
            return Result<List<DirectMessageResponseDto>>.NotFound(
                new Error(ErrorCodes.NotFound, "Receiver not found.")
            );
        }

        var messages = await dbContext.DirectMessages
            .Where(m =>
                (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.SentAt)
            .ThenBy(m => m.Id)
            .Select(m => new DirectMessageResponseDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                SentAt = m.SentAt
            })
            .AsNoTracking()
            .ToListAsync();

        return Result<List<DirectMessageResponseDto>>.Success(messages);
    }
}