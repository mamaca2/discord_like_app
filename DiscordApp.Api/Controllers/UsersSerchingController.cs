using DiscordApp.Application.Common.Paging;
using DiscordApp.Application.DTOs.UserFindingDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersSearchingController(IUserSearchService userSearchService) : BaseApiController
{
    // GET: api/users/search?query=Alex#00&pageNumber=1&pageSize=10
    [HttpGet("search")]
    [Authorize]
    public async Task<ActionResult<PagedList<UserSearchDto>>> SearchByUsername(
        [FromQuery] UserSearchQueryParams queryParams)
    {
        var result = await userSearchService.SearchByUsernameAsync(queryParams);
        return ToActionResult(result);
    }
}