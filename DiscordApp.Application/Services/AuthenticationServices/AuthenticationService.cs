using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiscordApp.Application.contracts;
using DiscordApp.Application.DTOs;
using DiscordApp.Application.DTOs.RegistrationDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DiscordApp.Application.Services.AuthenticationServices;

public class AuthenticationService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IAppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IEmailSender emailSender,
    IConfiguration configuration) : IAuthenticationService
{
    public async Task<Result> StartRegistrationAsync(StartRegistrationDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Email is already registered."));

        var existingPending = await dbContext.PendingRegistrations
            .Where(p => p.Email == dto.Email)
            .ToListAsync();
        dbContext.PendingRegistrations.RemoveRange(existingPending);

        var code = new Random().Next(100000, 999999).ToString();
        var hashedPassword = passwordHasher.HashPassword(new User(), dto.Password);

        var pending = new PendingRegistration
        {
            Email = dto.Email,
            PasswordHash = hashedPassword,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsVerified = false
        };

        dbContext.PendingRegistrations.Add(pending);
        await dbContext.SaveChangesAsync();

        await emailSender.SendEmailAsync(
            dto.Email,
            "Your verification code",
            $"<p>Your code is: <strong>{code}</strong>. It expires in 10 minutes.</p>");

        return Result.Success();
    }

    public async Task<Result> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var pending = await dbContext.PendingRegistrations
            .Where(p => p.Email == dto.Email)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (pending is null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, "No pending registration found for this email."));

        if (pending.ExpiresAt < DateTime.UtcNow)
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Code has expired."));

        if (pending.Code != dto.Code)
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Incorrect code."));

        pending.IsVerified = true;
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<RegisteredUserDto>> CompleteRegistrationAsync(CompleteRegistrationDto dto)
    {
        var pending = await dbContext.PendingRegistrations
            .Where(p => p.Email == dto.Email)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (pending is null)
            return Result<RegisteredUserDto>.NotFound(new Error(ErrorCodes.NotFound, "No pending registration found."));

        if (!pending.IsVerified)
            return Result<RegisteredUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, "Email not verified yet."));

        if (pending.ExpiresAt < DateTime.UtcNow)
            return Result<RegisteredUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, "Registration session expired. Please start again."));

        var generatedTag = await GenerateUniqueTagAsync(dto.Username);
        if (generatedTag is null)
            return Result<RegisteredUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, "All tags for this username are taken."));
   
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = pending.Email,
            UserName = $"{dto.Username}#{generatedTag}",
            DisplayName = dto.Username,
            Tag = generatedTag,
            Image = "default.png",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PasswordHash = pending.PasswordHash
        };

        IdentityResult createResult;
        try
        {
            createResult = await userManager.CreateAsync(user);
        }
        catch (Exception ex)
        {
            return Result<RegisteredUserDto>.BadRequest(
                new Error(ErrorCodes.BadRequest, $"Database user creation failed: {ex.InnerException?.Message ?? ex.Message}"));
        }

        if (!createResult.Succeeded)
        {
            var error = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<RegisteredUserDto>.BadRequest(new Error(ErrorCodes.BadRequest, error));
        }

        var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;

        if (!await roleManager.RoleExistsAsync(role))
        {
            role = "User";
        }

        await userManager.AddToRoleAsync(user, role);

        dbContext.PendingRegistrations.Remove(pending);
        await dbContext.SaveChangesAsync();

        return Result<RegisteredUserDto>.Success(new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            Username = user.DisplayName,
            Tag = user.Tag,
            Role = role
        });
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));

        var valid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));

        var token = await GenerateToken(user);
        return Result<string>.Success(token);
    }

    private async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JwtSettings:Key is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var duration = Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"]);
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
            .Where(u => u.DisplayName == username && u.Tag != null)
            .Select(u => u.Tag!)
            .ToListAsync();

        if (existingTags.Count >= 9999)
            return null;

        var existingSet = new HashSet<string>(existingTags);
        for (int i = 1; i <= 9999; i++)
        {
            var tag = i.ToString("D4");
            if (!existingSet.Contains(tag))
                return tag;
        }

        return null;
    }
}