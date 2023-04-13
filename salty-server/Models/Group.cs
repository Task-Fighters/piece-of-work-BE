namespace salty_server.Models;

public class Group
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public List<User> Users { get; set; }
    
    public List<Assignment> Assignments { get; set; }
}