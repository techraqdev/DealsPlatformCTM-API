using Deals.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public bool IsAdmin { get; set; }


    public AuthenticateResponse(User user, string token,bool isAdmin)
    {
        Id = user.UserId;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Email;
        Token = token;
        IsAdmin = isAdmin;
    }
}


public class AuthUser
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }


}