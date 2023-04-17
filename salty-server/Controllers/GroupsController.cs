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
    public async Task<ActionResult<Group>> Create(GroupDto groupDto)
    {
        var newGroup = new Group
        {
            Name = groupDto.Name
        };
        
        _context.Add(newGroup);
        await _context.SaveChangesAsync();
        return Ok(newGroup);
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
        var usersId = _context.GroupUser.Where(g => g.GroupsId == id).Select(u => u.UsersId).ToList();
        
        var response = new GroupResponseDto {
            Id = checkGroup.Id,
            Name = checkGroup.Name,
            UsersId = usersId,
            AssignmentsId = new List<int>()
        };

        return Ok(response);
    }
}