using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using salty_server.Models;

namespace salty_server.Controllers;

[ApiController]
[Route("[controller]")]

public class AssignmentsController : ControllerBase
{
    private readonly DbContext _context;

    public AssignmentsController(DbContext context)
    {
        _context = context;
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<AssignmentsResponseDTO>> CreateAssignment(AssignmentRequestDTO reqDto)
    {
        var assignmentCheck = await _context.Assignments.FirstOrDefaultAsync(g => g.Title == reqDto.Title && g.Group.Id == reqDto.GroupId);

        if (assignmentCheck != null)
        {
            return Conflict();
        }

        var group = await _context.Groups.FirstOrDefaultAsync(group => group.Id == reqDto.GroupId);

        if (group == null)
        {
            return NotFound();
        }

        var newAssignment = new Assignment()
        {
            Title = reqDto.Title,
            Description = reqDto.Description,
            StartDate = reqDto.StartDate,
            Group = group
        };

        _context.Add(newAssignment);
        await _context.SaveChangesAsync();
        var response = new AssignmentsResponseDTO()
        {
            Id = newAssignment.Id,
            Title = reqDto.Title,
            Description = reqDto.Description,
            StartDate = reqDto.StartDate,
            GroupId = reqDto.GroupId
        };
        return Ok(response);
    }

    [HttpGet, Authorize]
    [Route("group/{id}")]
    public ActionResult<List<AssignmentsResponseDTO>> GetAssignmentsByGroupId(int id)
    {
        return _context.Assignments
            .Where(a => a.Group.Id == id)
            .Select(a => new AssignmentsResponseDTO()
            {
                Id = a.Id,
                StartDate = a.StartDate,
                Title = a.Title,
                Description = a.Description,
                GroupId = a.Group.Id
            }).ToList();
    }

    [HttpGet, Authorize]
    public  ActionResult<List<AssignmentsResponseDTO>> GetAllAssignments()
    {
        return  _context.Assignments.Select(a => new AssignmentsResponseDTO()
        {
            Id = a.Id,
            StartDate = a.StartDate,
            Title = a.Title,
            Description = a.Description,
            GroupId = a.Group.Id 
        }).ToList();
    }

    [HttpGet, Authorize]
    [Route("{id}")]
    public async Task<ActionResult<AssignmentsResponseDTO>> GetAssignmentsById(int id)
    {
        var a = await _context.Assignments
            .Where(a => a.Id == id)
            .Select(a => new AssignmentsResponseDTO()
            {
                Id = a.Id,
                StartDate = a.StartDate,
                Title = a.Title,
                Description = a.Description,
                GroupId = a.Group.Id
            }).FirstOrDefaultAsync();

        if (a == null)
        {
            return NotFound();
        }

        return Ok(a);
    }

    [HttpPut, Authorize]
    [Route("{id}")]
    public async Task<ActionResult<AssignmentsResponseDTO>> UpdateAssignment(int id, [FromBody]AssignmentRequestDTO assignmentRequestDto)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id);
        if (assignment == null)
        {
            return NotFound();
        }

        var response = new AssignmentsResponseDTO()
        {
            Id = id,
            Title = assignmentRequestDto.Title,
            StartDate = assignmentRequestDto.StartDate,
            Description = assignmentRequestDto.Description,
            GroupId = assignmentRequestDto.GroupId
        };

        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == assignmentRequestDto.GroupId);

        if (group == null)
        {
            return NotFound("New Group not found");
        }

        assignment.Description = assignmentRequestDto.Description;
        assignment.Title = assignmentRequestDto.Title;
        assignment.StartDate = assignmentRequestDto.StartDate;
        assignment.Group = group;
        
        _context.Update(assignment);
        _context.SaveChangesAsync();

        return Ok(response);
    }  
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAssignment(int id)
    {
        var assignmentFound = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id);

        if (assignmentFound == null)
        {
            return NotFound("Assignment not Found");
        }

        _context.Assignments.Remove(assignmentFound);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}