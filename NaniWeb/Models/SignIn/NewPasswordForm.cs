using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.SignIn
{
    public class NewPasswordForm
    {
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[^\s]+$")]
        public string Password { get; set; }
    }
}