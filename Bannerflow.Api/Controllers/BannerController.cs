using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bannerflow.Api.Infrastructure;
using Bannerflow.Api.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace Bannerflow.Api.Controllers
{
    [Route("api/[controller]")]
    public class BannerController : Controller
    {

        private readonly List<Banner> Banners = new List<Banner> { new Banner { Id = 1, Created = DateTime.Now.AddDays(-1), Html = "<din>Banner</div>" } };
        
        public BannerController()
        {

        }
       
        // GET api/banner/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerByIdAsync(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

            var banner = Banners.FirstOrDefault(b => b.Id == id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(banner);
        }

        // POST api/banner
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]BannerCreateModel bannerCreateModel)
        {            
            var doc = new HtmlDocument();
            doc.LoadHtml(bannerCreateModel.Html);

            if (doc.ParseErrors.Any())
            {
                return BadRequest(doc.ParseErrors);
            }

            var banner = new Banner()
            {
                Html = bannerCreateModel.Html,
                Created = DateTime.Now
            };

            return Ok();
        }

        // PUT api/banner/5
        [HttpPut("{id}")]
        public Task<IActionResult> Put(int id, [FromBody]string value)
        {
            return null;
        }

        // DELETE api/banner/5
        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(int id)
        {
            return null;
        }
    }
}


/*GET: api/banner/{id}?type=html
 *POST: api/banner/
 * PUT: api/banner/
 * DELETE: api/banner/{id}
 */
