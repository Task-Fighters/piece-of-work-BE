using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ApiWithAuth;
using Microsoft.AspNetCore.Authorization;
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
    private readonly TokenService _tokenService;

    public UsersController(DbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }


    // GET: User
    [HttpGet, Authorize]
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
                Bootcamp = user.Bootcamp,
                GroupsId = groupsId
            };
        });

        return Ok(userDtoList);
    }

    // GET: User/Details/5
    [HttpGet("{id}"), Authorize]
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
            Bootcamp = user.Bootcamp,
            GroupsId = groupsId
        };

        return userRes;
    }


    [HttpPost, Authorize]
    public async Task<ActionResult<User>> AddUser(UserDto userDto)
    {
        var checkUser = await _context.Users
            .FirstOrDefaultAsync(m => m.Email == userDto.Email);
        if (checkUser != null)
        {
            return Conflict("user email already exist in the Database");
        }

        User newUser;
        UserResponseDto resDto;
        var allGroupsFound = true;
        var groups = userDto.GroupsId.Select(id =>
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
            {
                allGroupsFound = false;
            }

            return group;
        }).ToList();

        if (!allGroupsFound)
        {
            return NotFound("One or more of the groups provided does not exist yet");
        }
        newUser = new User
        {
            Email = userDto.Email,
            FullName = userDto.FullName,
            Role = userDto.Role,
            Location = userDto.Location,
            Status = userDto.Status,
            Bootcamp = userDto.Bootcamp,
            Groups = groups
        };

        resDto = new UserResponseDto
        {
            Email = userDto.Email,
            FullName = userDto.FullName,
            Role = userDto.Role,
            Location = userDto.Location,
            Status = userDto.Status,
            Bootcamp = userDto.Bootcamp,
            GroupsId = userDto.GroupsId
        };


        _context.Add(newUser);
        await _context.SaveChangesAsync();
        resDto.Id = newUser.Id;
        return Ok(resDto);
    }

    [HttpPut("login")]
    public async Task<ActionResult<UserLoginRes>> UserLogin(LoginDto loginDto)
    {
        var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

        if (UserFound == null)
        {
            return NotFound();
        }

        UserFound.Email = loginDto.Email;
        UserFound.GoogleId = loginDto.GoogleId;
        UserFound.ImageUrl = loginDto.ImageUrl;
        UserFound.FullName = loginDto.FullName;

        var accessToken = _tokenService.CreateToken(UserFound);
        var refreshToken = _tokenService.CreateRefreshToken(UserFound);

        _context.Update(UserFound);
        await _context.SaveChangesAsync();
        var userLoggedin = new UserLoginRes
        {
            Id = UserFound.Id,
            Token = accessToken,
            RefreshToken = refreshToken,
            Email = UserFound.Email,
            ImageUrl = UserFound.ImageUrl,
            FullName = UserFound.FullName,
            Role = UserFound.Role,
            Location = UserFound.Location,
            Status = UserFound.Status,
            Bootcamp = UserFound.Bootcamp
        };
        return Ok(userLoggedin);
    }


    [HttpPut("update/{id}"), Authorize]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody]UserDto userDto)
    {
        var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

        if (UserFound == null)
        {
            return NotFound();
        }


        UserFound.Email = userDto.Email;
        UserFound.Location = userDto.Location;
        UserFound.ImageUrl = userDto.ImageUrl;
        UserFound.FullName = userDto.FullName;
        UserFound.Role = userDto.Role;
        UserFound.Status = userDto.Status;
        UserFound.Bootcamp = userDto.Bootcamp;

        _context.Update(UserFound);
        await _context.SaveChangesAsync();

        var userResponse = new UserResponseDto
        {
            Id = UserFound.Id,
            GoogleId = UserFound.GoogleId,
            Email = UserFound.Email,
            FullName = UserFound.FullName,
            Role = UserFound.Role,
            Status = UserFound.Status,
            Location = UserFound.Location,
            ImageUrl = UserFound.ImageUrl,
            Bootcamp = UserFound.Bootcamp,
            GroupsId = userDto.GroupsId
        };

        return Ok(userResponse);
    }


    // GET: User/Delete/5
    [HttpDelete("{id}"), Authorize]
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

    [HttpGet, Authorize]
    [Route("refreshToken")]
    public async Task<ActionResult<string>> GetRefreshToken(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound("incorrect user Id provided");
        }
        return _tokenService.CreateToken(user);
    }
}