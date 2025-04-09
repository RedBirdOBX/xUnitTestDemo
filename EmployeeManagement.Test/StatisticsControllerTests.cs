using AutoMapper;
using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.Test
{
    public class StatisticsControllerTests
    {
        // Testing with HttpContext //
        // https://app.pluralsight.com/ilx/video-courses/d1a07995-8bbd-4124-a48f-b1f7f672091e/b2b14497-2e92-4035-b15c-206d0fcf2c2a/68224780-1aa6-4088-8ddd-22c9dce6b098
        // Gotta be honest - this feels recursive.  I am defining what the HttpContext will return and then
        // testing to see if it returned it.  I'm not sure how much value this has.

        /// <summary>
        /// Mocking the HttpContext.Features
        /// </summary>
        [Fact]
        public void GetStatistics_InputFromHttpConnectionFeature_MustReturnInputtedIPs()
        {
            // Arrange
            var localIPAddress = System.Net.IPAddress.Parse("111.111.111.111");
            var localPort = 5000;

            var remoteIPAddress = System.Net.IPAddress.Parse("222.222.222.222");
            var remotePort = 6000;

            var featureCollectionMock = new Mock<IFeatureCollection>();
            featureCollectionMock.Setup(x => x.Get<IHttpConnectionFeature>())
                                    .Returns(new HttpConnectionFeature
                                    {
                                        LocalIpAddress = localIPAddress,
                                        LocalPort = localPort,
                                        RemoteIpAddress = remoteIPAddress,
                                        RemotePort = remotePort
                                    });

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Features)
                                    .Returns(featureCollectionMock.Object);

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfiles.StatisticsProfile>());
            var mapper = new Mapper(mapperConfiguration);
            var statisticsController = new StatisticsController(mapper);

            statisticsController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };


            // Act
            var result = statisticsController.GetStatistics();

            // Assert
            var actionResult = Assert.IsType<ActionResult<StatisticsDto>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var statisticsDto = Assert.IsType<StatisticsDto>(okObjectResult.Value);

            Assert.Equal(localIPAddress.ToString(), statisticsDto.LocalIpAddress);
            Assert.Equal(localPort, statisticsDto.LocalPort);
            Assert.Equal(remoteIPAddress.ToString(), statisticsDto.RemoteIpAddress);
            Assert.Equal(remotePort, statisticsDto.RemotePort);
        }
    }
}
