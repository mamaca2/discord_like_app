using DiscordApp.Domain.Models;

namespace DiscordApp.Application.DTOs.ServerDTOs;

public static class InviteMappingExtensions
{
    public static ServerInviteDto ToDto(this ServerInvite invite)
    {
        return new ServerInviteDto
        {
            Code = invite.Code,
            ServerId = invite.ServerId,
            ServerName = invite.Server?.Name ?? string.Empty,
            CreatorId = invite.CreatorId,
            CreatorUsername = invite.Creator?.UserName ?? string.Empty,
            CreatedAt = invite.CreatedAt,
            ExpiresAt = invite.ExpiresAt,
            MaxUses = invite.MaxUses,
            Uses = invite.Uses,
            IsExpired = invite.IsExpired
        };
    }
}