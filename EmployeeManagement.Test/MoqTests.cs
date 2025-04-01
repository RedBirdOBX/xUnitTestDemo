using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Services.Test;
using Moq;

namespace EmployeeManagement.Test
{
    public class MoqTests
    {
        // non-moq'd example
        [Fact]
        public void FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated()
        {
            // Arrange
            var employeeManagementTestDataRepository = new EmployeeManagementTestDataRepository();

            //var employeeFactory = new EmployeeFactory();
            var employeeFactoryMock = new Mock<EmployeeFactory>();
            var employeeService = new EmployeeService(employeeManagementTestDataRepository, employeeFactoryMock.Object);

            // Act
            var employee = employeeService.FetchInternalEmployee(Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"));

            // Assert
            Assert.Equal(400, employee.SuggestedBonus);
        }

        // using moq with SetUp and Returns
        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_SuggestedBonusMustBeCalculated()
        {
            // Arrange
            var employeeManagementTestDataRepository = new EmployeeManagementTestDataRepository();
            var employeeFactoryMock = new Mock<EmployeeFactory>();

            // create the action of the moc'd method with SetUp.
            // It.IsAny<string> will give us random values instead hard-coding a test value.
            // SetUp builds the method.
            // Returns tells us what will be returned.
            employeeFactoryMock.Setup(m => m.CreateEmployee("Kevin", It.IsAny<string>(), null, false))
                                .Returns(new InternalEmployee("Kevin", "Dockx", 5, 2500, false, 1));

            employeeFactoryMock.Setup(m => m.CreateEmployee("Sandy", It.IsAny<string>(), null, false))
                                .Returns(new InternalEmployee("Sandy", "Dockx", 0, 3000, false, 1));

            // It.Is allows us to pass in a predicate
            employeeFactoryMock.Setup(m => m.CreateEmployee(It.Is<string>(value => value.Contains("a")), It.IsAny<string>(), null, false))
                                .Returns(new InternalEmployee("SomeoneWithAna", "Dockx", 0, 3000, false, 1));

            var employeeService = new EmployeeService(employeeManagementTestDataRepository, employeeFactoryMock.Object);

            // suggested bonus for new employees =
            // (years in service if > 0) * attended courses * 100
            //decimal suggestedBonus = 1000;

            // Act
            var employee1 = employeeService.CreateInternalEmployee("Kevin", "Dockx");
            var employee2 = employeeService.CreateInternalEmployee("Sandy", "Dockx");

            // Assert
            Assert.Equal(1000, employee1.SuggestedBonus);
            Assert.Equal(200, employee2.SuggestedBonus);
        }


        // example using real interface
        // the advantage here is that we do not need to create giant static test data files if we don't
        // want to.  We can mock the return of the **interface** method.
        [Fact]
        public void FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculatedViaInterface()
        {
            // Arrange
            var employeeManagementTestDataRepositoryMock = new Mock<IEmployeeManagementRepository>();

            // since we moq'd the interface, we need to set up the moq'd method.
            // we are stating that we will accept any quid as input and return our new employee as result.
            employeeManagementTestDataRepositoryMock.Setup(m => m.GetInternalEmployee(It.IsAny<Guid>()))
                                                    .Returns(new InternalEmployee("Kevin", "Dockx", 5, 2500, false, 1)
                                                    {
                                                        AttendedCourses = new List<Course>
                                                        {
                                                            new Course("Course1"),
                                                            new Course("Course2")
                                                        }
                                                    });

            var employeeFactoryMock = new Mock<EmployeeFactory>();
            var employeeService = new EmployeeService(employeeManagementTestDataRepositoryMock.Object, employeeFactoryMock.Object);

            // Act
            // we can pass in an empty guid - doesn't matter. The set up above defined the guid.
            var employee = employeeService.FetchInternalEmployee(Guid.Empty);

            // Assert
            Assert.Equal(1000, employee.SuggestedBonus);
        }


        // example using real interface
        // the advantage here is that we do not need to create giant static test data files if we don't
        // want to.  We can mock the return of the **interface** method.
        [Fact]
        public async Task FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculatedViaInterfaceWithAsync()
        {
            // Arrange
            var employeeManagementTestDataRepositoryMock = new Mock<IEmployeeManagementRepository>();

            // since we moq'd the interface, we need to set up the moq'd method.
            // we are stating that we will accept any quid as input and return our new employee as result.
            employeeManagementTestDataRepositoryMock.Setup(m => m.GetInternalEmployeeAsync(It.IsAny<Guid>()))
                                                    .ReturnsAsync(new InternalEmployee("Kevin", "Dockx", 5, 2500, false, 1)
                                                    {
                                                        AttendedCourses = new List<Course>
                                                        {
                                                            new Course("Course1"),
                                                            new Course("Course2")
                                                        }
                                                    });

            var employeeFactoryMock = new Mock<EmployeeFactory>();
            var employeeService = new EmployeeService(employeeManagementTestDataRepositoryMock.Object, employeeFactoryMock.Object);

            // Act
            // we can pass in an empty guid - doesn't matter. The set up above defined the guid.
            var employee = await employeeService.FetchInternalEmployeeAsync(Guid.Empty);

            // Assert
            Assert.Equal(1000, employee.SuggestedBonus);
        }

    }
}
