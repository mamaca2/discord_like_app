using DiscordApp.contracts;
using DiscordApp.controller;
using DiscordApp.DTOs.FriendRequestDTO;
using DiscordApp.DTOs.FriendRequestDTOs;
using DiscordApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DiscordApp.Controllers;

[ApiController]
[Route("api/friend-requests")]
[Authorize]
public class FriendRequestController(IFriendRequestService _friendRequestService) : BaseApiController
{
    [HttpPost("send")]
    public async Task<ActionResult<FriendRequestDto>> SendFriendRequest(CreateFriendRequestDto dto)
    {
        var senderId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (senderId is null)
            return Unauthorized();

        var result = await _friendRequestService.SendAsync(senderId, dto);
        return ToActionResult(result);
    }

    [HttpGet("incoming")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetIncomingPendingRequests()
    {
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (id is null)
            return Unauthorized();

        var result = await _friendRequestService.GetIncomingPendingRequestsAsync(id);
        return ToActionResult(result);
    }

    [HttpGet("outgoing")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetOutgoingPendingRequests()
    {
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (id is null)
            return Unauthorized();

        var result = await _friendRequestService.GetOutgoingPendingRequestsAsync(id);
        return ToActionResult(result);
    }

    [HttpPost("{requestId:int}/accept")]
    public async Task<IActionResult> AcceptFriendRequest(int requestId)
    {
        var receiverId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (receiverId is null)
            return Unauthorized();

        var result = await _friendRequestService.AcceptAsync(requestId, receiverId);
        return ToActionResult(result);
    }

    [HttpDelete("{requestId:int}/decline")]
    public async Task<IActionResult> DeclineFriendRequest(int requestId)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId is null)
            return Unauthorized();

        var result = await _friendRequestService.DeclineAsync(requestId, userId);
        return ToActionResult(result);
    }
}