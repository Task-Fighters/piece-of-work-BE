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
public class GroupsController : ControllerBase
{

    private readonly DbContext _context;

    public GroupsController(DbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Group>> CreateGroup(GroupDto groupDto)
    {
        var groupCheck = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupDto.Name);

        if (groupCheck != null)
        {
            return Conflict();
        }

        var newGroup = new Group
        {
            Name = groupDto.Name
        };

        _context.Add(newGroup);
        await _context.SaveChangesAsync();
        return Ok(newGroup);
    }

    [HttpGet]
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
            response.Add(new GroupResponseDto
            {
                Id = grp.Id,
                Name = grp.Name,
                Users = query.ToList(),
                AssignmentsId = new List<int>()
            });
        }
        return Ok(response);
    }

    [HttpGet]
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

        var response = new GroupResponseDto
        {
            Id = checkGroup.Id,
            Name = checkGroup.Name,
            Users = query.ToList(),
            AssignmentsId = new List<int>()
        };

        return Ok(response);
    }
}