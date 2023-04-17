using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace salty_server.Models;

public class GroupUser
{
    [Key, Column(Order = 2)] 
    public int UsersId { get; set; }
    
    public User User { get; set; }
    
    [Key, Column(Order = 1)] 
    public int GroupsId { get; set; }
    
    public Group Group { get; set; }
}