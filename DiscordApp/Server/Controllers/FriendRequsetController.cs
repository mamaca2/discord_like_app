using DiscordApp.contracts;
using DiscordApp.controller;
using DiscordApp.DTOs;
using DiscordApp.DTOs.FriendRequestDTO;
using DiscordApp.DTOs.FriendRequestDTOs;
using DiscordApp.Models;
using DiscordApp.Server.Models;
using DiscordApp.Server.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
namespace DiscordApp.Controllers;

[ApiController]
[Route("api/friend-requests")]
public class FriendRequestController(IFriendRequestService friendRequestService) : BaseApiController
{
    [HttpPost ("send-friend-request")]
    [Authorize]
    public async Task<ActionResult<FriendRequestDto>> SendFriendRequest(CreateFriendRequestDto dto)
    {
        var senderId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (senderId is null)
            return Unauthorized();

        var result = await friendRequestService.SendAsync(senderId, dto);
        return ToActionResult(result);
    }
}