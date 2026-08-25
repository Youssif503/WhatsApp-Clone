using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.DTOs;
using Whatsapp.DAL.data;

namespace Whatsapp.API.controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public UsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
            return Unauthorized(Response<string>.Fail("Unauthorized User"));

        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id != currentUserId)
            .OrderBy(user => user.First_Name)
            .ThenBy(user => user.Last_Name)
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.First_Name,
                LastName = user.Last_Name,
                Email = user.Email!,
                ImageUrl = user.ImageUrl
            })
            .ToListAsync();

        return Ok(Response<List<UserResponseDto>>.Success(users));
    }
}
