using System.ComponentModel.DataAnnotations;

namespace NaniWeb.Models.Users
{
    public class SelfEdit
    {
        [Required]
        [MinLength(3)]
        [RegularExpression(@"^[^\s]+$")]
        public string Username { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[^\s]+$")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[^\s]+$")]
        public string NewPassword { get; set; }
    }
}