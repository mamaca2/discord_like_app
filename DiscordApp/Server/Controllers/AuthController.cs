using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscordApp.Server.Models;
using DiscordApp.contracts;
using DiscordApp.DTOs;

namespace DiscordApp.controller;

[ApiController]
[Route("api/users")]
public class AuthController(IUsersService usersService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
    {
        var result = await usersService.RegisterAsync(registerUserDto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
    {
        var result = await usersService.LoginAsync(loginUserDto);
        return ToActionResult(result);
    }
}