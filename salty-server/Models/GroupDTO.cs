namespace salty_server.Models;

public class GroupDto
{
    public string Name { get; set; }
    
    public List<int> UserIds { get; set; }
}