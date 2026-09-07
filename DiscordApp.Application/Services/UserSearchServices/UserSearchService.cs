using DiscordApp.Application.Common.Paging;
using DiscordApp.Application.DTOs.UserFindingDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services.UserSearchServices;

public class UserSearchService(UserManager<User> userManager) : IUserSearchService
{
    public async Task<Result<PagedList<UserSearchDto>>> SearchByUsernameAsync(
        UserSearchQueryParams queryParams)
    {
        var validationResult = UserSearchQueryValidator.ParseAndValidate(queryParams.Query);
        if (!validationResult.IsSuccess)
        {
            return Result<PagedList<UserSearchDto>>.Failure(validationResult.Errors!);
        }

        var parsed = validationResult.Value!;

        if (string.IsNullOrEmpty(parsed.Username) && !parsed.HasTag)
        {
            return Result<PagedList<UserSearchDto>>.Success(
                new PagedList<UserSearchDto>([], 0, queryParams.PageNumber, queryParams.PageSize));
        }

        IQueryable<User> baseQuery;

        if (parsed.HasTag)
        {
            baseQuery = userManager.Users
                .AsNoTracking()
                .Where(u => u.DisplayName == parsed.Username && u.Tag != null);

            if (!string.IsNullOrEmpty(parsed.Tag))
            {
                baseQuery = baseQuery.Where(u => u.Tag!.StartsWith(parsed.Tag));
            }

            baseQuery = baseQuery.OrderBy(u => u.Tag);
        }
        else
        {
            baseQuery = userManager.Users
                .AsNoTracking()
                .Where(u => u.DisplayName != null && EF.Functions.ILike(u.DisplayName, $"%{parsed.Username}%"))
                .OrderBy(u => u.DisplayName)
                .ThenBy(u => u.Tag);
        }

        var projectedQuery = baseQuery.Select(u => new UserSearchDto
        {
            Id = u.Id,
            UserName = u.DisplayName!,
            Tag = u.Tag!,
            Image = u.Image
        });

        var pagedResult = await PagedList<UserSearchDto>.CreateAsync(
            projectedQuery, queryParams.PageNumber, queryParams.PageSize);

        return Result<PagedList<UserSearchDto>>.Success(pagedResult);
    }
}