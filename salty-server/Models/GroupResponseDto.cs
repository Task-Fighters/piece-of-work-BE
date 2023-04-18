namespace salty_server.Models;

public class GroupResponseDto
{
   
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> UsersId {get; set;}
    public List<int> AssignmentsId {get; set;} 
    
}