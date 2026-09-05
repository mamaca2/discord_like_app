using DiscordApp.Application.Common.Paging;
using DiscordApp.Application.DTOs.UserDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.Interfaces;

public interface IUserSearchService
{
    Task<Result<PagedList<UserSearchDto>>> SearchByUsernameAsync(UserSearchQueryParams queryParams);
}