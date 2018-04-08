using System;

namespace Bannerflow.Api.Models
{
    public class BannerDto
    {
        public Guid? Id { get; set; }
        public string Html { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
