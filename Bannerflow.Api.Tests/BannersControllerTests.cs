using AutoFixture;
using AutoMapper;
using Bannerflow.Api.Controllers;
using Bannerflow.Api.Data;
using Bannerflow.Api.Infrastructure;
using Bannerflow.Api.Models;
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
        public async void GetBannerByIdAsyncShouldReturnBannerDtoUnlessBannerDoesNotExist()
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

            //act1
            var resultFound = await sut.GetBannerByIdAsync(banner.Id);
            //act2
            var nonExistingId = Guid.NewGuid();
            var resultNotFound = await sut.GetBannerByIdAsync(nonExistingId);

            //assert resultFound
            var okObjectResult = Assert.IsType<OkObjectResult>(resultFound);

            mockRepo.Verify(mock => mock.GetAsync(banner.Id), Times.Once());

            var model = Assert.IsAssignableFrom<BannerDto>(
             okObjectResult.Value);

            Assert.Equal(model, bannerDto);
            Assert.Equal(200, okObjectResult.StatusCode);

            //assert resultNotFound
            var notFoundObjectResult = Assert.IsType<NotFoundResult>(resultNotFound);

            mockRepo.Verify(mock => mock.GetAsync(nonExistingId), Times.Once());
            Assert.Equal(404, notFoundObjectResult.StatusCode);
        }

        [Fact]
        public async void AddingNewBannerShouldReturnBannerDtoUnlessInvalidHtmlIsPassed()
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

            var createBannerDto = new BannerCreateDto {Html = "<div></div>" };

            //act resultValidHtml
            var resultValidHtml = await sut.PostAsync(createBannerDto);

            var createBannerDtoInvalid = new BannerCreateDto { Html = "<div>div>" };
            //act resultInvalidHtml
            var resultInvalidHtml = await sut.PostAsync(createBannerDtoInvalid);

            //assert resultValidHtml
            var okObjectResult = Assert.IsType<OkObjectResult>(resultValidHtml);

            mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Banner>()), Times.Once());

            var model = Assert.IsAssignableFrom<BannerDto>(
             okObjectResult.Value);

            Assert.Equal(model, bannerDto);
            Assert.Equal(200, okObjectResult.StatusCode);

            //assert invalidHtml
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(resultInvalidHtml);

            mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Banner>()), Times.Once());

            Assert.Equal(400, badRequestObjectResult.StatusCode);

        }
    }
}
