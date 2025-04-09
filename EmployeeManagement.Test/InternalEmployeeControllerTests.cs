using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace EmployeeManagement.Test
{
    public class InternalEmployeeControllerTests
    {
        private readonly InternalEmployeesController _internalEmployeesController;
        private readonly InternalEmployee _firstEmployee;

        public InternalEmployeeControllerTests()
        {

            _firstEmployee = new InternalEmployee("John", "Doe", 2, 3000, false, 2);

            // we can set this up per test or on a class level like seen here.
            var employeeServiceMock = new Mock<IEmployeeService>();
            employeeServiceMock.Setup(m => m.FetchInternalEmployeesAsync())
                                .ReturnsAsync(new List<InternalEmployee>()
                                {
                                    _firstEmployee,
                                    new InternalEmployee("Jamie", "Doe", 3, 3400, true, 1),
                                    new InternalEmployee("Jane", "Doe", 3, 4000, false, 3)
                                });

            // here's how you can set a mock'd automapper function but not recommended.
            // you'd be testing the mock and we want to test the real thing.
            //var autoMapperMock = new Mock<IMapper>();
            //// set up the map function from the source type to the destination type
            //autoMapperMock.Setup(m => m.Map<InternalEmployee, Models.InternalEmployeeDto>(It.IsAny<InternalEmployee>()))
            //                .Returns(new Models.InternalEmployeeDto());

            // better
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfiles.EmployeeProfile>();
            });
            var mapper = new Mapper(mapperConfig);

            // "But what is the correct approach here? Use a mock or an actual AutoMapper instance?
            // Well, if you're not testing the mapping behavior itself and you're strict,
            // you should **mock** autoMapper as well because your AutoMapper profiles can contain
            // behavior that you should test and you don't want errors in that behavior to
            // interfere with the actual behavior from the action that you're testing.
            // That's one school of thought, and that's what we've been doing throughout the course.
            // The other school of thought states that the mapping code, whether it's executed by
            // AutoMapper or some link, can be seen as part of the unit you're testing when
            // you're testing a controller action. Therefore, in those cases, it is okay to
            // use an actual mapper instead of a mock mapper."

            // set up the controller. requires employee service and automapper
            _internalEmployeesController = new InternalEmployeesController(employeeServiceMock.Object, mapper);
        }


        [Fact]
        public async Task GetInternalEmployees_GetAction_MustReturnOkResult()
        {
            // Arrange
            // set up what the employee service should return
            //var employeeServiceMock = new Mock<IEmployeeService>();
            //employeeServiceMock.Setup(m => m.FetchInternalEmployeesAsync())
            //                    .ReturnsAsync(new List<InternalEmployee>()
            //                    {
            //                        new InternalEmployee("John", "Doe", 2, 3000, false, 2),
            //                        new InternalEmployee("Jamie", "Doe", 3, 3400, true, 1),
            //                        new InternalEmployee("Jane", "Doe", 3, 4000, false, 3)
            //                    });

            //var internalEmployeesController = new InternalEmployeesController(employeeServiceMock.Object, null);

            // Act
            // controller returns an ActionResult
            var result = await _internalEmployeesController.GetInternalEmployees();

            // Assert
            // we need to inspect the result.Result property
            //var okResult = Assert.IsType<OkObjectResult>(result.Result);
            // OR

            // why do I need this step - declaring actionResult?
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);

            // why can't I just do this?
            //Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_MustReturnCollectionOfInternalEmployeeDtos()
        {
            // Arrange
            // done - constructor took care of this

            // Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            // Assert
            // this step gets the ActionResult but does seem unnecessary; result seems to already do that.
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);

            // cast the actionResult to OkObjectResult
            //.Value of the OkObjectResult is the actual data - the list of InternalEmployeeDtos
            // Controller is returning an IEnumerable<InternalEmployeeDto> which is technically an interface.
            // Assert.IsType will never work on an interface because Assert.IsType is an exact type check and seeing we get an interface back, using IsType will never be true.
            // So instead of using IsType, we use Assert.IsAssignableFrom
            Assert.IsAssignableFrom<IEnumerable<Models.InternalEmployeeDto>>(((OkObjectResult)actionResult.Result).Value);
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_ReturnsMatchingNumberOfInternalEmployees()
        {
            // Arrange
            // done - constructor took care of this

            // Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            // Assert
            // first, get a hold of the ActionResult
            var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);

            // cast the actionResult.Resulk to OkObjectResult... which should contain the model.. or list of models
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var countOfResults = ((IEnumerable<InternalEmployeeDto>)okResult.Value).Count();
            Assert.Equal(3, countOfResults);
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_ReturnsOkResultWithCorrectNumberOfInternalEmployees()
        {
            // combined, condensed technique //

            // Arrange
            // done - constructor took care of this

            // Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            // Assert

            var actionResultGetInternalEmployees = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);

            // Improvement:
            // IsType and IsAssignableFrom have RETURN values we can work with.
            // So we can rewrite our code so we reuse output from one assert statement
            // as input for another statement. Let's do that.
            // The result of the first IsType Assert is stored in ActionResult (from Controller).
            //var whatAmI = Assert.IsType<OkObjectResult>(actionResult.Result);

            // The result of ActionResult is passed through to the second IsType Assert, so we store the outcome in another variable, okObjectResult.
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResultGetInternalEmployees.Result);

            // In the next Assert, we then want to verify that the value of okObjectResult is assignable from an IEnumerable of our InternalEmployeeDto,
            // so instead of all this casting here, we now pass through the result of the previous Assert.
            // Then we again store the result of IsAssignableFrom in another variable because what this results in is our set of DTOs.
            var dtos = Assert.IsAssignableFrom<IEnumerable<Models.InternalEmployeeDto>>(okObjectResult.Value);

            // And we can then use the Assert.Equal statement without all this casting by simply passing in the variable and counting the items in it.
            Assert.Equal(3,dtos.Count());
        }

        [Fact]
        public async Task GetInternalEmployees_GetAction_MappingFieldsWasAccurate()
        {
            // Arrange
            // done - constructor took care of this

            // Act
            var result = await _internalEmployeesController.GetInternalEmployees();

            // assert
            var actionResultGetInternalEmployees = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResultGetInternalEmployees.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<Models.InternalEmployeeDto>>(okObjectResult.Value);
            var firstDto = dtos.FirstOrDefault();

            Assert.Equal(firstDto.FirstName, _firstEmployee.FirstName);
            Assert.Equal(firstDto.LastName, _firstEmployee.LastName);
            Assert.Equal(firstDto.Salary, _firstEmployee.Salary);
            Assert.Equal(firstDto.YearsInService, _firstEmployee.YearsInService);
            Assert.Equal(firstDto.SuggestedBonus, _firstEmployee.SuggestedBonus);
        }
    }
}
