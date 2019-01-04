using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NaniWeb.Models.Settings
{
    public class FcmForm
    {
        [Required] public bool EnableFcm { get; set; }

        [Required] public string FcmApiKey { get; set; }

        [Required] public string FcmProjectId { get; set; }

        [Required] public ulong FcmSenderId { get; set; }

        [Required] public IFormFile FcmKeyFile { get; set; }
    }
}