using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.SignIn
{
    public class SignInForm
    {
        [Required] [MinLength(3)] public string Username { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required] public bool Remember { get; set; }
    }
}