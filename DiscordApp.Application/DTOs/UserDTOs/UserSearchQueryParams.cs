using DiscordApp.Application.Common.Paging;

namespace DiscordApp.Application.DTOs.UserDTOs;

public class UserSearchQueryParams : PageParams
{
    public string? Query { get; set; }
}