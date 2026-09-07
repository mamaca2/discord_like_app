using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace DiscordApp.Application.Validators;

public class CustomUserValidator : IUserValidator<User>
{
    public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var errors = new List<IdentityError>();

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidEmail",
                Description = "Email cannot be empty."
            });
        }
        else
        {
            var owner = await manager.FindByEmailAsync(user.Email);
            if (owner != null && owner.Id != user.Id)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = $"Email '{user.Email}' is already taken."
                });
            }
        }

        return errors.Count == 0
            ? IdentityResult.Success
            : IdentityResult.Failed([.. errors]);
    }
}