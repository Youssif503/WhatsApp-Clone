using Microsoft.AspNetCore.Identity;
namespace Whatsapp.DAL.models;
public class User:IdentityUser
{
    public string First_Name { get; set; }
    public string Last_Name { get; set; } 
    public string? ImageUrl { get; set; }
}