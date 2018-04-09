using AutoFixture;
using AutoMapper;
using Bannerflow.Api.Controllers;
using Bannerflow.Api.Data;
using Bannerflow.Api.Infrastructure;
using Bannerflow.Api.Models;
using Bannerflow.Api.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bannerflow.Api.Tests
{
    public class BannersControllerTests
    {

        [Fact]
        public async void GetBannersAsyncShouldReturnListOfBannerDto()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();

            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banners = fixture.Create<IEnumerable<Banner>>();

            mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(banners)).Verifiable();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();


            var bannersDto = fixture.Create<List<BannerDto>>();
            mockMapper.Setup(mapper => mapper.Map<List<Banner>, List<BannerDto>>(It.IsAny<List<Banner>>())).Returns(bannersDto).Verifiable();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var result = await sut.GetBannersAsync();

            //assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            var model = Assert.IsAssignableFrom<List<BannerDto>>(
               okObjectResult.Value);

            mockRepo.Verify(mock => mock.GetAllAsync(), Times.Once());
            mockMapper.Verify(mock => mock.Map<List<Banner>, List<BannerDto>>(It.IsAny<List<Banner>>()), Times.Once);

            Assert.Equal(model.Count(), bannersDto.Count());
            Assert.Equal(200, okObjectResult.StatusCode);


        }
        [Fact]
        public async void GetBannersAsyncShouldReturnEmptyListIfNoBannersExists()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();

            IEnumerable<Banner> banners = new List<Banner>();

            mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(banners)).Verifiable();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var bannersDto = new List<BannerDto>();
            mockMapper.Setup(mapper => mapper.Map<List<Banner>, List<BannerDto>>(It.IsAny<List<Banner>>())).Returns(bannersDto).Verifiable();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var result = await sut.GetBannersAsync();

            //assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            mockRepo.Verify(mock => mock.GetAllAsync(), Times.Once());

              var model = Assert.IsAssignableFrom<List<BannerDto>>(
               okObjectResult.Value);

            Assert.True(!model.Any());
            Assert.Equal(200, okObjectResult.StatusCode);


        }
        [Fact]
        public async void GetBannerByIdAsyncShouldReturnBannerDto()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();

            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banner = fixture.Create<Banner>();

            mockRepo.Setup(repo => repo.GetAsync(banner.Id)).Returns(Task.FromResult(banner)).Verifiable();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var bannerDto = fixture.Create<BannerDto>();
            mockMapper.Setup(mapper => mapper.Map<Banner, BannerDto>(It.IsAny<Banner>())).Returns(bannerDto).Verifiable();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var resultFound = await sut.GetBannerByIdAsync(banner.Id);
            

            //assert
            var okObjectResult = Assert.IsType<OkObjectResult>(resultFound);

            mockRepo.Verify(mock => mock.GetAsync(banner.Id), Times.Once());
            mockMapper.Verify(mock => mock.Map<Banner, BannerDto>(banner), Times.Once);

            var model = Assert.IsAssignableFrom<BannerDto>(
             okObjectResult.Value);

            Assert.Equal(model, bannerDto);
            Assert.Equal(200, okObjectResult.StatusCode);

        }

        [Fact]
        public async void GetBannerByIdAsyncShouldReturnNotFoundResult()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();

          
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();
            
            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);
           
            //act
            var resultNotFound = await sut.GetBannerByIdAsync(It.IsAny<Guid>());

            //assert
            var notFoundObjectResult = Assert.IsType<NotFoundResult>(resultNotFound);

            mockRepo.Verify(mock => mock.GetAsync(It.IsAny<Guid>()), Times.Once());

            Assert.Equal(404, notFoundObjectResult.StatusCode);
        }

        [Fact]
        public async void AddingNewBannerShouldReturnBannerDtoWhenPassedValidHtml()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();

            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banner = fixture.Create<Banner>();

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Banner>())).Returns(Task.FromResult(banner)).Verifiable();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var bannerDto = fixture.Create<BannerDto>();
            mockMapper.Setup(mapper => mapper.Map<Banner, BannerDto>(It.IsAny<Banner>())).Returns(bannerDto).Verifiable();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            
            var validHtml = "<div></div>";
            var createBannerDto = fixture.Build<BannerCreateDto>().With(b => b.Html, validHtml).Create();


            //act 
            var resultValidHtml = await sut.PostAsync(createBannerDto);
            
            //assert 
            var okObjectResult = Assert.IsType<OkObjectResult>(resultValidHtml);

            mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Banner>()), Times.Once());

            var model = Assert.IsAssignableFrom<BannerDto>(
             okObjectResult.Value);

            Assert.Equal(model, bannerDto);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.Equal(sut.ControllerContext.HttpContext.Response.Headers["Location"], $"http://localhost:50211/api/v1/banners/{bannerDto.Id}");
        }

        [Fact]
        public async void AddingNewBannerShouldReturnBadRequestIfInvalidHtml()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var fixture = new Fixture();
            var invalidHtml = "<div>div>";
            var createBannerDto = fixture.Build<BannerCreateDto>().With(b => b.Html, invalidHtml).Create();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var resultInvalidHtml = await sut.PostAsync(createBannerDto);

            //assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(resultInvalidHtml);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
        }

        [Fact]
        public async void UpdatingBannerShouldReturnBannerDto()
        {
            //arrange
            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banner = fixture.Create<Banner>();

            var mockRepo = new Mock<IBannerRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).Returns(Task.FromResult(banner)).Verifiable();
            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), banner)).Returns(Task.FromResult(true)).Verifiable();

            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var bannerDto = fixture.Create<BannerDto>();
            mockMapper.Setup(mapper => mapper.Map<Banner, BannerDto>(It.IsAny<Banner>())).Returns(bannerDto).Verifiable();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var validHtml = "<div></div>";
            var bannerUpdateDtoValid = fixture.Build<BannerUpdateDto>().With(b => b.Html, validHtml).Create();

            //act 
            var resultValidHtml = await sut.PutAsync(It.IsAny<Guid>(), bannerUpdateDtoValid);


            //assert 
            var okObjectResult = Assert.IsType<OkObjectResult>(resultValidHtml);
            mockRepo.Verify(mock => mock.UpdateAsync(It.IsAny<Guid>(), banner), Times.Once());
            var model = Assert.IsAssignableFrom<BannerDto>(
             okObjectResult.Value);
            Assert.Equal(model, bannerDto);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.Equal(sut.ControllerContext.HttpContext.Response.Headers["Location"], $"http://localhost:50211/api/v1/banners/{banner.Id}");

        }

        [Fact]
        public async void UpdatingBannerShouldReturnBadRequestWhenPassedInvalidHtml()
        {
            //arrange
            var mockRepo = new Mock<IBannerRepository>();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);
            var fixture = new Fixture();
            var invalidHtml = "<div>div>";
            var bannerUpdateDtoInvalid = fixture.Build<BannerUpdateDto>().With(b => b.Html, invalidHtml).Create();

            //act
            var resultInvalidHtml = await sut.PutAsync(It.IsAny<Guid>(), bannerUpdateDtoInvalid);
            
            //assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(resultInvalidHtml);
            Assert.IsAssignableFrom<IEnumerable<HtmlParseError>>(
             badRequestObjectResult.Value);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
        }

        [Fact]
        public async void RemoveBannerRequestShouldReturnStatusCode204IfBannerExists()
        {
            //arrange
            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banner = fixture.Create<Banner>();
            var mockRepo = new Mock<IBannerRepository>();
            mockRepo.Setup(repo => repo.GetAsync(banner.Id)).Returns(Task.FromResult(banner)).Verifiable();
            mockRepo.Setup(repo => repo.RemoveAsync(banner.Id)).Returns(Task.FromResult(true)).Verifiable();

            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var result = await sut.DeleteAsync(banner.Id);

            //assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            mockRepo.Verify(mock => mock.RemoveAsync(banner.Id), Times.Once());
            mockRepo.Verify(mock => mock.GetAsync(banner.Id), Times.Once());
            Assert.Equal(204, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void RemoveBannerRequestShouldReturnNotFoundResultIfBannerDoesNotExist()
        {
            //arrange

            var mockRepo = new Mock<IBannerRepository>();
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object);

            //act
            var result = await sut.DeleteAsync(It.IsAny<Guid>());

            //assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            mockRepo.Verify(mock => mock.RemoveAsync(It.IsAny<Guid>()), Times.Never());            

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void GetBannerHtmlShouldReturnString()
        {
            //arrange  
            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId).With(ba => ba.Html, "<div></div>"));
            var banner = fixture.Create<Banner>();

            var mockRepo = new Mock<IBannerRepository>();
            mockRepo.Setup(repo => repo.GetAsync(banner.Id)).Returns(Task.FromResult(banner)).Verifiable();
            
            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //act
            var result = await sut.GetBannerHtmlByIdAsync(banner.Id);

            //assert
            Assert.IsType<string>(result);
            mockRepo.Verify(mock => mock.GetAsync(banner.Id), Times.Once());
            Assert.Equal(200, sut.HttpContext.Response.StatusCode);
            Assert.Equal(banner.Html, result);
        }

        [Fact]
        public async void GetBannerHtmlShouldReturnNotFoundResult()
        {
            //arrange            

            var mockRepo = new Mock<IBannerRepository>();

            var mockLogger = new Mock<ILogger<BannersController>>();
            var mockMapper = new Mock<IMapper>();
            var mockTransformer = new Mock<ITransformer>();

            var sut = new BannersController(mockRepo.Object, mockLogger.Object, mockMapper.Object, mockTransformer.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //act
            var result = await sut.GetBannerHtmlByIdAsync(It.IsAny<Guid>());
            
            //assert
            var model = Assert.IsType<string>(result);
            mockRepo.Verify(mock => mock.GetAsync(It.IsAny<Guid>()), Times.Once());
            
            Assert.Equal(404, sut.HttpContext.Response.StatusCode);
            Assert.Equal(model, string.Empty);
        }

    }
}
