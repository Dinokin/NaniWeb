using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Profile
{
    public class NewPasswordForm
    {
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}