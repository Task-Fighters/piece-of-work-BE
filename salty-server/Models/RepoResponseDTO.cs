namespace salty_server.Models;

public class RepoResponseDTO
{
    public int Id { get; set; }
    public string Url { get; set; }
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
}