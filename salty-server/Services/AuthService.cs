using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ApiWithAuth;

public class AuthService
{
    private readonly DbContext _context;
    public AuthService(DbContext context)
    {
        _context = context;
    }
    public static string SubjectId(ClaimsPrincipal user)
    {
        return user?.Claims?.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;
    }
    public async Task<string> getUserRole(ClaimsPrincipal userClaim)
    {
        var email = SubjectId(userClaim);
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        if (user == null)
        {
            return "not found";
        }
        return user.Role;
    }
}