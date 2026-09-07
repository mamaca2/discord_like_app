using DiscordApp.Application.DTOs;
using DiscordApp.Application.DTOs.RegistrationDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.contracts;

public interface IAuthenticationService
{
    Task<Result> StartRegistrationAsync(StartRegistrationDto dto);
    Task<Result> VerifyCodeAsync(VerifyCodeDto dto);
    Task<Result<RegisteredUserDto>> CompleteRegistrationAsync(CompleteRegistrationDto dto);
    Task<Result<string>> LoginAsync(LoginUserDto dto);
}