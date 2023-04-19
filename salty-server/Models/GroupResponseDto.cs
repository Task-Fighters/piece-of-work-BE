namespace salty_server.Models;

public class GroupResponseDto
{
   
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserDetail> Users {get; set;}
    public List<AssignmentDetails> AssignmentsId {get; set;} 
    
}