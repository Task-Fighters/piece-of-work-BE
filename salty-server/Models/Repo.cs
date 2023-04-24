namespace salty_server.Models;

public class Repo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public Assignment Assignment { get; set; }
    public User User { get; set; }
}