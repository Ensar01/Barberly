using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Barberly.Database.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateOnly RegistrationDate { get; set; }
    }
}