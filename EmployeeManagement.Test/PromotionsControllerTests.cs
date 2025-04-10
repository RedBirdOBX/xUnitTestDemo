using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Models;
using EmployeeManagement.Services.Test;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class PromotionsControllerTests
    {
        // mocking the http call entirely
        // waaayyyyy too complicated - try building the custom message handlers per call/test.
        [Fact]
        public async Task CreatePromotion_RequestPromotionForEligibleEmployee_MustPromoteEmployee()
        {
            // Arrange
            var expetedEmployeeId = Guid.NewGuid();
            var currentJobLevel = 1;
            var promotioForCreationDto = new PromotionForCreationDto
            {
                EmployeeId = expetedEmployeeId,
            };

            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(x => x.FetchInternalEmployeeAsync(It.IsAny<Guid>()))
                                .ReturnsAsync(new InternalEmployee("Anna", "Johnson", 3, 3400, true, currentJobLevel)
                                {
                                    Id = expetedEmployeeId,
                                    SuggestedBonus = 500
                                });


            // handler mock
            var eligibleForPromotionHandlerMock = new Mock<HttpMessageHandler>();
            eligibleForPromotionHandlerMock.Protected()
                                            .Setup<Task<HttpResponseMessage>>(
                                                "SendAsync",
                                                ItExpr.IsAny<HttpRequestMessage>(),
                                                ItExpr.IsAny<CancellationToken>()
                                            )
                                            .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                                            {
                                                Content = new StringContent(
                                                                            JsonSerializer.Serialize(
                                                                                                    new PromotionEligibility() { EligibleForPromotion = true },
                                                                                                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                                                                                                    ),
                                                                            Encoding.ASCII,
                                                                            "application/json"
                                                                            )
                                            });


            var httpClient = new HttpClient(eligibleForPromotionHandlerMock.Object);
            var promotionService = new PromotionService(httpClient, new EmployeeManagementTestDataRepository());

            var promtionController = new PromotionsController(employeeServiceMock.Object, promotionService);

            // Act
            var result = await promtionController.CreatePromotion(promotioForCreationDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var promotionResultDto = Assert.IsType<PromotionResultDto>(okObjectResult.Value);
            Assert.Equal(expetedEmployeeId, promotionResultDto.EmployeeId);
            Assert.Equal(++currentJobLevel, promotionResultDto.JobLevel);
        }
    }
}
