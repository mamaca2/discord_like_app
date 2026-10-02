using System.ComponentModel.DataAnnotations;

namespace DiscordApp.Application.DTOs.ServerDTOs.InviteDTOs;

public class CreateInviteDto
{
    /// <summary>
    /// Optional expiration time in hours. Null means the invite never expires.
    /// </summary>
    [Range(1, 168, ErrorMessage = "Duration must be between 1 hour and 168 hours (7 days).")]
    public int? DurationHours { get; set; }

    /// <summary>
    /// Maximum times this invite can be used. 0 or null means unlimited uses.
    /// </summary>
    [Range(0, 1000, ErrorMessage = "Max uses must be between 0 (unlimited) and 1000.")]
    public int? MaxUses { get; set; } = 0;
}