using DiscordApp.Application.DTOs.AuthenticationDTOs;
using DiscordApp.Application.DTOs.RegistrationDTOs;

namespace DiscordApp.Application.contracts;

public interface IAuthenticationService
{
    Task<Result> StartRegistrationAsync(StartRegistrationDto dto);
    Task<Result> VerifyCodeAsync(VerifyCodeDto dto);
    Task<Result<RegisteredUserDto>> CompleteRegistrationAsync(CompleteRegistrationDto dto);
    Task<Result<string>> LoginAsync(LoginUserDto dto);
}