using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace EmployeeManagement.Test
{
    public class DemoInternalEmployeesControllerTests
    {
        // MODEL STATE DEMO
        // Normally, the [ApiController] attribute is used and it will take care of all the
        // ModelState validation for you. In the event that you cannot have the [ApiController]
        // attribute on the controller, you can use the following code to validate the ModelState.

        [Fact]
        public async Task CreateInternalEmployee_InvalidInput_MustReturnBadRequest()
        {
            // Arrange
            // for this test, we do not need the functionality of the svc or automapper.
            // we're just mocking because the controller requires them.
            var employeeServiceMock = new Mock<IEmployeeService>();
            var mapperMock = new Mock<IMapper>();
            var controller = new DemoInternalEmployeesController(employeeServiceMock.Object, mapperMock.Object);

            // We are newing up DTO with an empty string as FirstName, so this is effectively an invalid DTO
            // we're passing through. The thing is though this won't do the trick. Remember, we are running
            // these tests in isolation. The **model binder** does not execute. And seeing it's during the
            // binding process that the ModelState is potentially made invalid, this won't work.
            // This effectively will not result in an invalid ModelState. What we pass through as
            // values for the InternalEmployeeForCreationDto properties, thus, simply doesn't matter.
            var internalEmployeeDto = new InternalEmployeeForCreationDto()
            {
                FirstName = string.Empty,
                LastName = "Doe"
            };

            // How do we make the ModelState invalid then? Well, we can add a model error to it.
            // The ModelState is approachable via the ModelState dictionary on the controller.
            controller.ModelState.AddModelError("FirstName", "First name is required.");

            // Act
            var result = await controller.CreateInternalEmployee(internalEmployeeDto);

            // Assert
            // make sure it is an ActionResult
            var actionResult = Assert.IsType<ActionResult<Models.InternalEmployeeDto>>(result);

            // make sure it is a BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

            // make sure the error message has state
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void GetProtectedInternalEmployees_GetActionForUserInAdminRole_MustRedirectToGetInternalEmployeesOnProtectedInternalEmployees()
        {
            // Arrange
            var employeeServiceMock = new Mock<IEmployeeService>();
            var mapperMock = new Mock<IMapper>();
            var demoInternalEmployeesController = new DemoInternalEmployeesController(employeeServiceMock.Object, mapperMock.Object);

            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Karen"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "UnitTest");

            // A ClaimsPrincipal in .NET represents the security context of a user,
            // including their identity and any associated claims.
            // It is part of the claims-based authentication model, which is widely
            // used in modern applications to manage user authentication and authorization.
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            };

            demoInternalEmployeesController.ControllerContext = new ControllerContext() { HttpContext = httpContext };

            // Act
            var result = demoInternalEmployeesController.GetProtectedInternalEmployees();

            // Assert
            // This controller returns an IActionResult which is slightly easier to
            // work with than ActionResult<T>.
            // We can cast those directly to the actual resulting types we want to test for.
            // Remember...interfaces use IsAssignableFrom.
            var actionResult = Assert.IsAssignableFrom<IActionResult>(result);

            // (We can do one or the other... or both)

            // We can do is directly check whether we're dealing with the type we expect,
            // a RedirectToAction result.
            var redirectoToActionResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("GetInternalEmployees",redirectoToActionResult.ActionName);
            Assert.Equal("ProtectedInternalEmployees", redirectoToActionResult.ControllerName);
        }

        /// <summary>
        /// mocking the whole thing
        /// </summary>
        [Fact]
        public void GetProtectedInternalEmployees_GetActionForUserInAdminRole_MustRedirectToGetInternalEmployeesOnProtectedInternalEmployees_WithMoq()
        {
            // Arrange
            var employeeServiceMock = new Mock<IEmployeeService>();
            var mapperMock = new Mock<IMapper>();
            var demoInternalEmployeesController = new DemoInternalEmployeesController(employeeServiceMock.Object, mapperMock.Object);

            // mocking the claims principle
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(x => x.IsInRole(It.Is<string>(s => s == "Admin"))).Returns(true);

            // mocking the HttpContext
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(mockPrincipal.Object);

            demoInternalEmployeesController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = demoInternalEmployeesController.GetProtectedInternalEmployees();

            // Assert
            var actionResult = Assert.IsAssignableFrom<IActionResult>(result);
            var redirectoToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetInternalEmployees", redirectoToActionResult.ActionName);
            Assert.Equal("ProtectedInternalEmployees", redirectoToActionResult.ControllerName);
        }
    }
}
