using Microsoft.AspNetCore.Mvc;
using System;

namespace Bannerflow.Api.Controllers
{
    public class GetBannerByIdParameters
    {
        [FromRoute]
        public Guid BannerId { get; set; }
    }
}
