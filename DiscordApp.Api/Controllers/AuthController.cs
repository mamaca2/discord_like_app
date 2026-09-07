using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs;
using DiscordApp.Application.DTOs.RegistrationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace DiscordApp.controller;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IAuthenticationService authService) : BaseApiController
{
    [HttpPost("start-registration")]
    public async Task<IActionResult> StartRegistration(StartRegistrationDto dto)
    {
        var result = await authService.StartRegistrationAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode(VerifyCodeDto dto)
    {
        var result = await authService.VerifyCodeAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("complete-registration")]
    public async Task<ActionResult<RegisteredUserDto>> CompleteRegistration(CompleteRegistrationDto dto)
    {
        var result = await authService.CompleteRegistrationAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
    {
        var result = await authService.LoginAsync(loginUserDto);
        return ToActionResult(result);
    }
}