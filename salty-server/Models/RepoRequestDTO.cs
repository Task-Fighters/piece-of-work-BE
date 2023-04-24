namespace salty_server.Models;

public class RepoRequestDTO
{
    public string Url { get; set; }
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
}