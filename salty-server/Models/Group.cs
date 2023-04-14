namespace salty_server.Models;

public class Group
{
    public Group(){
        Users = new HashSet<User>();
    }
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual ICollection<User>? Users { get; set; }
    
    public List<Assignment>? Assignments { get; set; }
}