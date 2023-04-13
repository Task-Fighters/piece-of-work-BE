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
    public async Task<ActionResult<Group>> Create(GroupDTO group)
    {
        var newGroup = new Group
        {
            Name = group.Name
        };
        
        _context.Add(newGroup);
        await _context.SaveChangesAsync();
        return Ok(newGroup);
    }
}