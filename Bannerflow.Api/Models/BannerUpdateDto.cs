using System.ComponentModel.DataAnnotations;

namespace Bannerflow.Api.Models
{
    public class BannerUpdateDto
    {
        [Required]
        public string Html { get; set; }
    }
}
