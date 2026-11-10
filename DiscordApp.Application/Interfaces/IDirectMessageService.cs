using System;
using DiscordApp.Application.DTOs.ChatDTOs;

namespace DiscordApp.Application.Interfaces;

public interface IDirectMessageService
{
    Task<Result> SendMessageAsync(string senderId, DirectMessageDTO directMessageDto);
    Task<Result<List<DirectMessageResponseDto>>> GetConversationAsync(string senderId, string receiverId);
}
