using DiscordApp.Application.Common.Paging;
using DiscordApp.Application.DTOs.UserDTOs;
using DiscordApp.Application.Interfaces;
using DiscordApp.Application.Results;
using DiscordApp.Domain.constants;
using DiscordApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscordApp.Application.Services.UserSearchServices;

public class UserSearchService(UserManager<User> userManager) : IUserSearchService
{
    public async Task<Result<PagedList<UserSearchDto>>> SearchByUsernameAsync(
        UserSearchQueryParams queryParams)
    {
        if (string.IsNullOrWhiteSpace(queryParams.Query))
        {
            return Result<PagedList<UserSearchDto>>.Success(
                new PagedList<UserSearchDto>([], 0, queryParams.PageNumber, queryParams.PageSize));
        }

        var trimmedQuery = queryParams.Query.Trim();
        IQueryable<User> baseQuery;

        if (trimmedQuery.Contains('#'))
        {
            var parts = trimmedQuery.Split('#', 2);
            var searchName = parts[0];
            var searchTag = parts[1];

            if (string.IsNullOrEmpty(searchTag))
            {
                baseQuery = userManager.Users
                    .Where(u => u.UserName == searchName && u.Tag != null)
                    .OrderBy(u => u.Tag);
            }
            else
            {
                baseQuery = userManager.Users
                    .Where(u => u.UserName == searchName && u.Tag != null && u.Tag.StartsWith(searchTag))
                    .OrderBy(u => u.Tag);
            }
        }
        else
        {
            baseQuery = userManager.Users
                .Where(u => u.UserName != null && u.UserName.ToLower().StartsWith(trimmedQuery.ToLower()))
                .OrderBy(u => u.UserName)
                .ThenBy(u => u.Tag);
        }

        var projectedQuery = baseQuery.Select(u => new UserSearchDto
        {
            Id = u.Id,
            UserName = u.UserName!,
            Tag = u.Tag!,
            Image = u.Image
        });

        var pagedResult = await PagedList<UserSearchDto>.CreateAsync(
            projectedQuery, queryParams.PageNumber, queryParams.PageSize);

        return Result<PagedList<UserSearchDto>>.Success(pagedResult);
    }
}