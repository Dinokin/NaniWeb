using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.SignIn
{
    public class ResetPasswordForm
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}