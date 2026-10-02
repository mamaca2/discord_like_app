using System.Security.Claims;
using DiscordApp.Application.DTOs.ServerDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.Api.Controllers;

[ApiController]
[Route("api/servers")]
[Authorize]
public class ServersController(IServerService serverService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ServerDto>> CreateServer([FromBody] CreateServerDto createServerDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await serverService.CreateServerAsync(userId, createServerDto);
        return ToActionResult(result);
    }
    
}