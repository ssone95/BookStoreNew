using System.ComponentModel.DataAnnotations;

namespace IdentityServerOld.Data.Models
{
    public class LoginViewModelBase
    {

        [Required] 
        public string Username { get; set; }
        [Required] 
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }
    }
}