namespace salty_server.Models;

public class UserLoginRes
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    
    public string RefreshToken { get; set; }

    public string Email { get; set; }

    public string? FullName { get; set; }

    public string Role { get; set; }

    public string Location { get; set; }

    public string? ImageUrl { get; set; }

    public string Status { get; set; }

    public string? Bootcamp { get; set; }
}
