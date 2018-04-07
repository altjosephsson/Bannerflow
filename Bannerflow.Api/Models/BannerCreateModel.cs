using System.ComponentModel.DataAnnotations;

namespace Bannerflow.Api.Models
{
    public class BannerCreateModel
    {
        [Required]
        public string Html { get; set; }
    }
}
