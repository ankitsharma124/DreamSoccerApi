using System.ComponentModel.DataAnnotations;

namespace DreamSoccerApi.Dtos.User
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password {get; set;}
    }
}