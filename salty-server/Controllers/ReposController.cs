using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using salty_server.Models;

namespace salty_server.Controllers;
[ApiController]
[Route("[controller]")]
public class ReposController : ControllerBase
{
    private readonly DbContext _context;

    public ReposController(DbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<RepoResponseDTO>> CreateRepo(RepoRequestDTO reqDto)
    {
        var repoCheck = await _context.Repos.FirstOrDefaultAsync(r => r.Url == reqDto.Url && r.Assignment.Id == reqDto.AssignmentId);

        if (repoCheck != null)
        {
            return Conflict("There's a submission with the same Url for this assignment");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == reqDto.UserId);
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == reqDto.AssignmentId);
        if (user == null || assignment == null)
        {
            return NotFound("User or Assignment not found");
        }

        var newRepo = new Repo()
        {
            Url = reqDto.Url,
            Assignment = assignment,
            User = user
        };

        _context.Add(newRepo);
        await _context.SaveChangesAsync();
        var response = new RepoResponseDTO()
        {
            Id = newRepo.Id,
            Url = newRepo.Url,
            AssignmentId = reqDto.AssignmentId,
            UserId = reqDto.UserId
        };
        return Ok(response);
    }

    [HttpGet]
    [Route("Assignment/{id}")]
    public  ActionResult<List<RepoResponseDTO>> GetReposForAssignment(int id)
    {
        return _context.Repos.Where(r => r.Assignment.Id == id)
            .Select(r => new RepoResponseDTO()
            {
                Id = r.Id,
                Url = r.Url,
                UserId = r.User.Id,
                AssignmentId = id
            }).ToList();
    }
    
    [HttpGet]
    [Route("User/{id}")]
    public  ActionResult<List<RepoResponseDTO>> GetReposForUser(int id)
    {
        return _context.Repos.Where(r => r.User.Id == id)
            .Select(r => new RepoResponseDTO()
            {
                Id = r.Id,
                Url = r.Url,
                UserId = id,
                AssignmentId = r.Assignment.Id
            }).ToList();
    }
}