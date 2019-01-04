using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.SignIn
{
    public class NewPasswordForm
    {
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}