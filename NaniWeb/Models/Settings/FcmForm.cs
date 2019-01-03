using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NaniWeb.Models.Settings
{
    public class FcmForm
    {
        [Required] public bool EnableFcm { get; set; }

        public string FcmApiKey { get; set; }

        public string FcmProjectId { get; set; }

        public ulong FcmSenderId { get; set; }
        
        public IFormFile FcmKeyFile { get; set; }
    }
}