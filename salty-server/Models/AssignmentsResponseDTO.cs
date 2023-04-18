namespace salty_server.Models;

public class AssignmentsResponseDTO
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public string Description { get; set; }
    
    public int GroupId { get; set; }
}