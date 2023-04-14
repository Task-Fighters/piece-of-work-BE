namespace salty_server.Models;

public class User
{
    public User(){
        Groups = new HashSet<Group>();
    }
    public int Id { get; set; }
    
    public string? GoogleId { get; set; }
    
    public string Email { get; set; }
    
    public string? FullName { get; set; }
    
    public string Role { get; set; }
    
    public string Location { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string Status { get; set; }
    
    public virtual ICollection<Group>? Groups { get; set; }

}
