using System.ComponentModel.DataAnnotations;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Dtos.User
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public RoleEnum Role { get; set; }
    }
}