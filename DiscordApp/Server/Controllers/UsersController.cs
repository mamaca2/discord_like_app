using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
	private AppDbContext _appDbContext;
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
}
