using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using salty_server.Models;

namespace salty_server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DbContext _context;

    public UserController(DbContext context)
    {
        _context = context;
    }

    // GET: User
    [HttpGet]
    public async Task<ActionResult<List<User>>> Index()
    {
        return _context.Users != null ?
            (await _context.Users.ToListAsync()) :
            Problem("Entity set 'DbContext.User'  is null.");
    }

    // GET: User/Details/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Details(int? id)
    {
        if (id == null || _context.Users == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return user;
    }


    [HttpPost]
    public async Task<ActionResult<User>> Create(UserDto userDto)
    {
        var checkUser = await _context.Users
            .FirstOrDefaultAsync(m => m.Email == userDto.Email);

        if (checkUser != null)
        {
            return Conflict();
        }

        var newUser = new User
        {
            Email = userDto.Email,
            FullName = userDto.FullName,
            Role = userDto.Role,
            Location = userDto.Location,
            Status = userDto.Status
        };

        _context.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(newUser);
        // return Ok(userDto);
    }

    [HttpPut("/login")]
    public async Task<ActionResult<User>> Login(LoginDto loginDto)
    {
        var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

        if (UserFound == null)
        {
            return NotFound();
        }

        UserFound.Email = loginDto.Email;
        UserFound.GoogleId = loginDto.GoogleId;
        UserFound.ImageUrl = loginDto.ImageUrl;

        _context.Update(UserFound);
        await _context.SaveChangesAsync();
        return Ok(UserFound);
        // return Ok(loginDto);

    }


    // GET: User/Delete/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

        if (UserFound == null)
        {
            return NotFound();
        }

        _context.Users.Remove(UserFound);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}