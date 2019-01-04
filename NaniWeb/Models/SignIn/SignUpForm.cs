using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.SignIn
{
    public class SignUpForm
    {
        [Required] [MinLength(3)] public string Username { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}