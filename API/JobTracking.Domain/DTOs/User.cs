﻿namespace JobTracking.Domain.DTOs;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string LastName { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }

}