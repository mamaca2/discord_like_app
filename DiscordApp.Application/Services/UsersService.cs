using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs;
using DiscordApp.Application.DTOs.UserDTOs;
using DiscordApp.Application.Results;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DiscordApp.Application.Services;

public class UsersService(
    UserManager<User> userManager,
    IConfiguration configuration) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto dto)
    {
        // 1. Check if Email is taken
        var existingEmail = await userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            return Result<RegisteredUserDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "Email is already registered."));
        }

        // 2. Generate a unique 4-digit tag for this exact UserName
        var generatedTag = await GenerateUniqueTagAsync(dto.Username);
        if (generatedTag is null)
        {
            return Result<RegisteredUserDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, "All available tags for this username have been taken."));
        }

        // 3. Create User entity with assigned Tag
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Username,
            Tag = generatedTag,
            Image = "default.png"
        };

        var createResult = await userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.First().Description;
            return Result<RegisteredUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, error));
        }

        // 4. Assign default role and return RegisteredUserDto
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;
        await userManager.AddToRoleAsync(user, role);

        return Result<RegisteredUserDto>.Success(new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.UserName!,
            Tag = user.Tag!,
            Role = role
        });
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
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        // Add roles
        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(
            roles.Select(role =>
                new Claim(
                    ClaimTypes.Role,
                    role)));

        var key = configuration["JwtSettings:Key"];

        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException(
                "JwtSettings:Key is not configured.");
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var duration = Convert.ToInt32(
            configuration["JwtSettings:DurationInMinutes"]);

        var expiresAt = DateTime.UtcNow.AddMinutes(duration);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string?> GenerateUniqueTagAsync(string username)
    {
        var existingTags = await userManager.Users
            .Where(u => u.UserName == username && u.Tag != null)
            .Select(u => u.Tag!)
            .ToListAsync();

        if (existingTags.Count >= 9999)
            return null;

        var existingSet = new HashSet<string>(existingTags);

        for (int i = 1; i <= 9999; i++)
        {
            var tag = i.ToString("D4");
            if (!existingSet.Contains(tag))
            {
                return tag;
            }
        }

        return null;
    }
}