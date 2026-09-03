using DiscordApp.constants;
using DiscordApp.contracts;
using DiscordApp.DTOs;
using DiscordApp.DTOs.UserDTOs;
using DiscordApp.Server.Models;
using DiscordApp.Server.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DiscordApp.Services;

public class UsersService(
    UserManager<User> userManager,
    IConfiguration configuration) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(
        RegisterUserDto registerUserDto)
    {
        var user = new User
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Username
        };

        var result = await userManager.CreateAsync(
            user,
            registerUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new Error(
                    ErrorCodes.BadRequest,
                    e.Description))
                .ToArray();

            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        var roleResult = await userManager.AddToRoleAsync(
            user,
            registerUserDto.Role);

        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors
                .Select(e => new Error(
                    ErrorCodes.BadRequest,
                    e.Description))
                .ToArray();

            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            Role = registerUserDto.Role
        };
        Console.WriteLine("------------------------", registeredUser.Username, "----------------------------");
        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
        {
            return Result<string>.Failure(
                new Error(
                    ErrorCodes.BadRequest,
                    "Invalid credentials."));
        }

        var valid = await userManager.CheckPasswordAsync(
            user,
            dto.Password);

        if (!valid)
        {
            return Result<string>.Failure(
                new Error(
                    ErrorCodes.BadRequest,
                    "Invalid credentials."));
        }

        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    private async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(
            role => new Claim(ClaimTypes.Role, role));

        claims.AddRange(roleClaims);

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                configuration["JwtSettings:Key"] ?? string.Empty));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(
                    configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result<UserSearchDto>> FindByUsernameAndTagAsync(
        UserLookupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) ||
            string.IsNullOrWhiteSpace(dto.Tag))
        {
            return Result<UserSearchDto>.BadRequest(
                new Error(
                    ErrorCodes.BadRequest,
                    "Username and tag are both required."));
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(
                u => u.UserName == dto.UserName &&
                     u.Tag == dto.Tag);

        if (user is null)
        {
            return Result<UserSearchDto>.NotFound(
                new Error(
                    ErrorCodes.NotFound,
                    "No user found with that username and tag."));
        }

        return Result<UserSearchDto>.Success(
            new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName!
            });
    }

    public Task<Result<List<UserSearchDto>>> SearchByUsernameAsync(
        string username)
    {
        throw new NotImplementedException();
    }
}

