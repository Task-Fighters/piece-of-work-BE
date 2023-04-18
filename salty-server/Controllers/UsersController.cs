using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using salty_server.Models;

namespace salty_server.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly DbContext _context;

    public UsersController(DbContext context)
    {
        _context = context;
    }


    // GET: User
    [HttpGet]
    public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
    {

        var users = await _context.Users.ToListAsync();
        var userDtoList = users.Select(user =>
        {
            var groupsId = _context.GroupUser.Where(g => g.UsersId == user.Id).Select(g => g.GroupsId).ToList();

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Location = user.Location,
                Status = user.Status,
                GroupsId = groupsId
            };
        });

        return Ok(userDtoList);
    }

    // GET: User/Details/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(int? id)
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

        var groupsId = _context.GroupUser
        .Where(g => g.UsersId == id)
        .Select(g => g.GroupsId).ToList();


        var userRes = new UserResponseDto()
        {
            Id = user.Id,
            GoogleId = user.GoogleId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Location = user.Location,
            Status = user.Status,
            ImageUrl = user.ImageUrl,
            GroupsId = groupsId
        };

        return userRes;
    }


    [HttpPost]
    public async Task<ActionResult<User>> AddUser(UserDto userDto)
    {
        var checkUser = await _context.Users
            .FirstOrDefaultAsync(m => m.Email == userDto.Email);

        var checkGroup = await _context.Groups.FirstOrDefaultAsync(group => group.Id == userDto.GroupId);

        if (checkUser != null)
        {
            return Conflict();
        }

        User newUser;
        UserResponseDto resDto;

        if (checkGroup != null)
        {
            ICollection<Group> groups = new Collection<Group>();
            groups.Add(checkGroup);
            var groupsId = groups.Select(r => r.Id).ToList();
            newUser = new User
            {
                Email = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role,
                Location = userDto.Location,
                Status = userDto.Status,
                Groups = groups
            };

            resDto = new UserResponseDto
            {
                Email = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role,
                Location = userDto.Location,
                Status = userDto.Status,
                GroupsId = groupsId
            };
        }
        else
        {
            newUser = new User
            {
                Email = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role,
                Location = userDto.Location,
                Status = userDto.Status
            };

            resDto = new UserResponseDto
            {
                Email = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role,
                Location = userDto.Location,
                Status = userDto.Status,
                GroupsId = new List<int>()
            };
        }

        _context.Add(newUser);
        await _context.SaveChangesAsync();
        resDto.Id = newUser.Id;
        return Ok(resDto);
    }

    [HttpPut("login")]
    public async Task<ActionResult<User>> UserLogin(LoginDto loginDto)
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
    }



    // GET: User/Delete/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
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