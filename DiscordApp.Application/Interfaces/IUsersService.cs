using DiscordApp.Application.DTOs;
using DiscordApp.Application.DTOs.UserDTOs;
using DiscordApp.Application.Results;

namespace DiscordApp.Application.contracts;

public interface IUsersService
{
    Task<Result<string>> LoginAsync(LoginUserDto dto);
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
}