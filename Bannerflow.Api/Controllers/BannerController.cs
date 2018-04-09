using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bannerflow.Api.Data;
using Bannerflow.Api.Infrastructure;
using Bannerflow.Api.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Bannerflow.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class BannersController : Controller
    {
        private readonly ILogger<BannersController> _logger;
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;
        private readonly ITransformer _transformer;

        private readonly EventId _eventId = new EventId(101, "Bannerflow.Api");
        private readonly Guid _customerErrorCode = Guid.NewGuid();

        public BannersController(IBannerRepository bannerRepository,
            ILogger<BannersController> logger,
            IMapper mapper,
            ITransformer transformer)
        {
            _bannerRepository = bannerRepository ?? throw new ArgumentNullException(nameof(bannerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
        }

        // GET api/banners/
        [HttpGet]
        public async Task<IActionResult> GetBannersAsync()
        {
            try
            {
                var banners = await _bannerRepository.GetAllAsync();    
              
                var bannersDto = _mapper.Map<List<Banner>, List<BannerDto>>(banners.ToList());

                return Ok(bannersDto);
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                return StatusCode(500, $"Error code: {_customerErrorCode}");
            }


        }
       
        // GET api/banners/{id}?fields={fieldName}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerByIdAsync(Guid id, [FromQuery] string[] fields = null)
        {
            Banner banner;

            try
            {
                banner = await _bannerRepository.GetAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                return StatusCode(500, $"Error code: {_customerErrorCode}");
            }            

            if(banner == null)
            {
                return NotFound();
            }

            BannerDto bannerDto;

            if (fields == null || !fields.Any())
            {
                bannerDto = _mapper.Map<Banner, BannerDto>(banner);
                return Ok(bannerDto);
            }

            //would be interesting to see another solution than this
            bannerDto = _transformer.Transform(banner, fields);                 

            return Ok(bannerDto);
        }

        // POST api/banners
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> PostAsync([FromBody]BannerCreateDto bannerCreateModel)
        {            
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(bannerCreateModel.Html);

                if (doc.ParseErrors.Any())
                {
                    return BadRequest(doc.ParseErrors);
                }

                var banner = new Banner
                {
                    Html = bannerCreateModel.Html,
                    Created = DateTime.UtcNow,
                    Id = Guid.NewGuid()
                };                          

                var resultBanner =  await _bannerRepository.AddAsync(banner);
                
                _logger.LogInformation(_eventId, $"Succesfully created resource id: {resultBanner.Id}");

                var bannerDto = _mapper.Map<Banner, BannerDto>(resultBanner);

                //Get url location from another source
                Request.HttpContext.Response.Headers.Add("Location", $"http://localhost:50211/api/v1/banners/{bannerDto.Id}");

                return Ok(bannerDto);
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                return StatusCode(500, $"Error code: {_customerErrorCode}");
            }        
                                  
        }

        // PUT api/banners/{id}
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody]BannerUpdateDto bannerUpdateDto)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(bannerUpdateDto.Html);

                if (doc.ParseErrors.Any())
                {
                    return BadRequest(doc.ParseErrors);
                }

                var banner = await _bannerRepository.GetAsync(id);
                if (banner == null)
                {
                    return NotFound();
                }            

                banner.Html = bannerUpdateDto.Html;
                banner.Modified = DateTime.UtcNow;

                var result = await _bannerRepository.UpdateAsync(banner.Id, banner);

                if (!result)
                {
                    throw new Exception($"Unable to update banner id: {banner.Id}");
                }

                //Get url location from another source
                Request.HttpContext.Response.Headers.Add("Location", $"http://localhost:50211/api/v1/banners/{banner.Id}");

                var bannerDto = _mapper.Map<Banner, BannerDto>(banner);

                _logger.LogInformation(_eventId, $"Succesfully updated resource id: {banner.Id}");

                return Ok(bannerDto);
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                return StatusCode(500, $"Error code: {_customerErrorCode}");
            }
         
        }

        // DELETE api/banners/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var banner = await _bannerRepository.GetAsync(id);
                if (banner == null)
                {
                    return NotFound();
                }

                var result = await _bannerRepository.RemoveAsync(id);

                if (!result)
                {
                    throw new Exception($"Unable to delete banner id: {id}");
                }

                _logger.LogInformation(_eventId, $"Succesfully deleted resource id: {banner.Id}");

                return StatusCode(204);
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                return StatusCode(500, $"Error code: {_customerErrorCode}");
            }           
        }

        // GET api/banners/{id}/html
        [HttpGet("{id}/html")]
        [Produces("text/html")]
        public async Task<string> GetBannerHtmlByIdAsync(Guid id)
        {
            try
            {
                var banner = await _bannerRepository.GetAsync(id);
                if (banner == null)
                {
                    Request.HttpContext.Response.StatusCode = 404;
                    return string.Empty;
                }
                Request.HttpContext.Response.StatusCode = 200;
                return banner.Html;
            }
            catch (Exception e)
            {
                _logger.LogCritical(_eventId, e, $"ErrorCode: {_customerErrorCode}");
                Request.HttpContext.Response.StatusCode = 500;
                return string.Empty;
            }     
        }
    }
}