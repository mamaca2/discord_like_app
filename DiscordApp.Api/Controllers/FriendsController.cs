using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs.FriendDTOs;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace DiscordApp.Controllers;

[ApiController]
[Route("api/friends")]
[Authorize]
public class FriendsController(IFriendsService _friendsService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<FriendDto>>> GetFriends()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _friendsService.GetFriendsListAsync(userId);
        return ToActionResult(result);
    }

    [HttpDelete("{friendId}")]
    public async Task<IActionResult> Unfriend(string friendId)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _friendsService.UnfriendAsync(userId, friendId);
        return ToActionResult(result);
    }
}