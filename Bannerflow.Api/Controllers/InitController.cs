using Bannerflow.Api.Data;
using Bannerflow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerflow.Api.Controllers
{
    [Route("api/[controller]")]
    public class InitController : Controller
    {
        private readonly IBannerRepository _bannerepository;

        public InitController(IBannerRepository bannerepository)
        {
            _bannerepository = bannerepository ?? throw new ArgumentNullException(nameof(bannerepository));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            await _bannerepository.RemoveAllBannersAsync();
            var name = await _bannerepository.CreateIndexAsync();
            await _bannerepository.AddAsync(new Banner() { Id = Guid.NewGuid(), Html = "<div>Hello1</div>", Created = DateTime.UtcNow });
            await _bannerepository.AddAsync(new Banner() { Id = Guid.NewGuid(), Html = "<div>Hello2</div>", Created = DateTime.UtcNow });
            await _bannerepository.AddAsync(new Banner() { Id = Guid.NewGuid(), Html = "<div>Hello3</div>", Created = DateTime.UtcNow });
            await _bannerepository.AddAsync(new Banner() { Id = Guid.NewGuid(), Html = "<div>Hello4</div>", Created = DateTime.UtcNow });

            return Ok("Banners removed and created.");
        }
    }
}