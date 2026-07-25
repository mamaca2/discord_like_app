using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscordApp.Server.Models;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public UsersController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _appDbContext.Users.ToListAsync();

        return Ok(users);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser(User newUser)
    {
        var userWithSameName = await _appDbContext.Users
            .FirstOrDefaultAsync(existingUser =>
                existingUser.Username == newUser.Username);

        if (userWithSameName != null)
        {
            return Conflict("Username already exists.");
        }

        _appDbContext.Users.Add(newUser);

        await _appDbContext.SaveChangesAsync();

        return Ok(newUser);
    }
}