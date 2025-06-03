using JobTracking.DataAccess.Data.Base;

namespace JobTracking.DataAccess.Data.Models;

public class User : Entity
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string LastName { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
    
    public string Role { get; set; }
}