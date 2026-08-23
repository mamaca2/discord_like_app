using DiscordApp.contracts;
using DiscordApp.controller;
using DiscordApp.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUsersService usersService) : BaseApiController
{
    [HttpGet("find-by-tag")]
    [Authorize]
    public async Task<ActionResult<UserSearchDto>> FindByUsernameAndTag(UserLookupDto dto)
    {
        var result = await usersService.FindByUsernameAndTagAsync(dto);
        return ToActionResult(result);
    }
}