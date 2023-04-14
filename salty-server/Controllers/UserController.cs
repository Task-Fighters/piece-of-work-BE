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
        return _context.User != null ?
            (await _context.User.ToListAsync()) :
            Problem("Entity set 'DbContext.User'  is null.");
    }

    // GET: User/Details/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Details(int? id)
    {
        if (id == null || _context.User == null)
        {
            return NotFound();
        }

        var user = await _context.User
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return user;
    }


    [HttpPost]
    public async Task<ActionResult<User>> Create([Bind("GoogleId,Email,FullName,Role,Location,Image,Status")] UserDTO user)
    {
        var checkUser = await _context.User
            .FirstOrDefaultAsync(m => m.Email == user.Email);

        if (checkUser != null)
        {
            return Conflict();
        }

        var newUser = new User
        {
            GoogleId = "test",
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Location = user.Location,
            Image = user.Image,
            Status = user.Status,
            GroupId = user.GroupId
        };

        _context.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(newUser);
    }

    [HttpPut("/login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
    {
        var UserFound = await _context.User.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

        if (UserFound == null)
        {
            return NotFound();
        }

        UserFound.Email = loginDto.Email;
        UserFound.GoogleId = loginDto.GoogleId;
        UserFound.Image = loginDto.Image;

        _context.Update(UserFound);
        await _context.SaveChangesAsync();
        return Ok(UserFound);

    }


    // GET: User/Edit/5
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Edit(int? id)
    {
        if (id == null || _context.User == null)
        {
            return NotFound();
        }

        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    // GET: User/Delete/5
    [HttpDelete]
    public async Task<ActionResult<User>> Delete(int? id)
    {
        if (id == null || _context.User == null)
        {
            return NotFound();
        }

        var user = await _context.User
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }
}