using System;
using DiscordApp.Application.DTOs.ChatDTOs;

namespace DiscordApp.Application.Interfaces;

public interface IDirectMessageService
{
    Task<Result> SendMessage(string senderId, DirectMessageDTO directMessageDto);
}
