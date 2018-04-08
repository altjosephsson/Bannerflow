using System.ComponentModel.DataAnnotations;

namespace Bannerflow.Api.Models
{
    public class BannerCreateDto
    {
        [Required]
        public string Html { get; set; }
    }
}
