using DiscordApp.DTOs;
using DiscordApp.DTOs.UserDTOs;
using DiscordApp.Server.Results;

namespace DiscordApp.contracts;

public interface IUsersService
{
    Task<Result<string>> LoginAsync(LoginUserDto dto);
    Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    Task<Result<List<User>>> SearchByUsernameAsync(UserSearchDto dto);
}