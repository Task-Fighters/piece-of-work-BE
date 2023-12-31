using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
public class GroupsController : ControllerBase
{

    private readonly DbContext _context;
    private readonly AuthService _authService;

    public GroupsController(DbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<GroupResponseDto>> CreateGroup(GroupDto groupDto)
    {
        var userRole =  await _authService.getUserRole(HttpContext.User);

        if (userRole.ToLower() != "admin")
        {
            return Unauthorized("You are not an admin!!!!! >:(");
        }
        var groupCheck = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupDto.Name);

        if (groupCheck != null)
        {
            return Conflict();
        }
        
        var newGroup = new Group
        {
            Name = groupDto.Name
        };

        var userDetailsList = new List<UserDetail>();
        
        _context.Add(newGroup);

        foreach (var id in groupDto.UserIds)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var groupUser = new GroupUser()
            {
                UsersId = id,
                User = user,
                GroupsId = newGroup.Id,
                Group = newGroup
            };
            var userDetail = new UserDetail()
            {
                Id = id,
                Name = user.FullName
            };
            userDetailsList.Add(userDetail);
            
            _context.Add(groupUser);
        }
        
        await _context.SaveChangesAsync();
        var response = new GroupResponseDto()
        {
            Id = newGroup.Id,
            Name = newGroup.Name,
            Users = userDetailsList,
            AssignmentsId = new List<AssignmentDetails>()
        };
        return Ok(response);
    }

    [HttpGet, Authorize]
    public async Task<ActionResult<List<GroupResponseDto>>> GetAllGroups()
    {
        var allGroups = await _context.Groups.ToListAsync();

        if (allGroups == null)
        {
            return NotFound();
        }

        var response = new List<GroupResponseDto>();

        foreach (var grp in allGroups)
        {
            var query =
            from g in _context.GroupUser where g.GroupsId == grp.Id
            join user in _context.Users on  g.UsersId equals user.Id
            select new UserDetail
            {
                Id = user.Id,
                Name = user.FullName
            };

            var assignmentDetails = _context.Assignments
                .Where(a => a.Group.Id == grp.Id)
                .Select(a => new AssignmentDetails()
                {
                    Id = a.Id,
                    Title = a.Title
                }).ToList();
            
            response.Add(new GroupResponseDto
            {
                Id = grp.Id,
                Name = grp.Name,
                Users = query.ToList(),
                AssignmentsId = assignmentDetails
            });
        }
        return Ok(response);
    }

    [HttpGet, Authorize]
    [Route("{id}")]
    public async Task<ActionResult<GroupResponseDto>> GetGroupById(int id)
    {
        var checkGroup = await _context.Groups.FirstOrDefaultAsync(group => group.Id == id);

        if (checkGroup == null)
        {
            return NotFound();
        }

        var query =
        from g in _context.GroupUser where g.GroupsId == id
        join user in _context.Users on  g.UsersId equals user.Id
        select new UserDetail
        {
            Id = user.Id,
            Name = user.FullName
        };

        var assignmentDetails = _context.Assignments
            .Where(a => a.Group.Id == id)
            .Select(a => new AssignmentDetails()
            {
                Id = a.Id,
                Title = a.Title
            })
            .ToList();

        var response = new GroupResponseDto
        {
            Id = checkGroup.Id,
            Name = checkGroup.Name,
            Users = query.ToList(),
            AssignmentsId = assignmentDetails
        };

        return Ok(response);
    }


    [HttpPut]
    public async Task<ActionResult> UpdateGroup(GroupUpdateDto newGroup)
    {
        var userRole =  await _authService.getUserRole(HttpContext.User);

        if (userRole.ToLower() != "admin")
        {
            return Unauthorized("You are not an admin!!!!! >:(");
        }
        var GroupFound = await _context.Groups.FirstOrDefaultAsync(group => group.Id == newGroup.Id);

        if (GroupFound == null)
        {
            return NotFound("Group not Found");
        }

        GroupFound.Name = newGroup.Name;
 

        _context.Update(GroupFound);
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    //need cascade delete
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGroup(int id)
    {
        var userRole =  await _authService.getUserRole(HttpContext.User);

        if (userRole.ToLower() != "admin")
        {
            return Unauthorized("You are not an admin!!!!! >:(");
        }
        var groupFound = await _context.Groups.FirstOrDefaultAsync(group => group.Id == id);

        if (groupFound == null)
        {
            return NotFound("Group not Found");
        }

        _context.Groups.Remove(groupFound);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost, Authorize]
    [Route("{id}/AddUser")]
    public async Task<ActionResult>AddUser(int id, List<int> usersId)
    {
        var userRole =  await _authService.getUserRole(HttpContext.User);

        if (userRole.ToLower() != "admin")
        {
            return Unauthorized("You are not an admin!!!!! >:(");
        }
        var groupFound = await _context.Groups.FirstOrDefaultAsync(group => group.Id == id);

        if (groupFound == null)
        {
            return NotFound("Group not Found");
        }
        
        foreach (var userId in usersId)
        {
            var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (UserFound == null)
            {
                return NotFound("User not found");
            }

            var groupUser = new GroupUser
            {
                UsersId = userId,
                GroupsId = id,
                User = UserFound,
                Group = groupFound
            };

            var groupUserExist = await _context.GroupUser.FirstOrDefaultAsync(gUser =>
                gUser.UsersId == groupUser.UsersId && gUser.GroupsId == groupUser.GroupsId);

            if (groupUserExist == null)
            {
                _context.Add(groupUser);
            }
        }
        
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete, Authorize]
    [Route("{id}/RemoveUser")]
    public async Task<ActionResult> RemoveUser(int id, List<int> usersId)
    {
        var userRole =  await _authService.getUserRole(HttpContext.User);

        if (userRole.ToLower() != "admin")
        {
            return Unauthorized("You are not an admin!!!!! >:(");
        }
        var groupFound = await _context.Groups.FirstOrDefaultAsync(group => group.Id == id);

        if (groupFound == null)
        {
            return NotFound("Group not Found");
        }

        foreach (var userId in usersId)
        {
            var UserFound = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (UserFound == null)
            {
                return NotFound("User not found");
            }
            
            var groupUserExist = await _context.GroupUser.FirstOrDefaultAsync(gUser =>
                gUser.UsersId == userId && gUser.GroupsId == id);

            if (groupUserExist != null)
            {
                _context.Remove(groupUserExist);
            }
        }
        
        await _context.SaveChangesAsync();

        return NoContent();
    }
}