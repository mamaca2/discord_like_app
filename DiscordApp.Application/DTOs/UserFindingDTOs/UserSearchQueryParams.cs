using DiscordApp.Application.Common.Paging;

namespace DiscordApp.Application.DTOs.UserFindingDTOs;

public class UserSearchQueryParams : PageParams
{
    public string? Query { get; set; }
}