using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Connection.Authentication
{
    public class LoginInfo
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}