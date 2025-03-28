using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.DbContexts;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Services.Test;
using EmployeeManagement.Test.HttpMessageHandlers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace EmployeeManagement.Test
{
    public class TestIsolationApproachesTests
    {
        // GOAL: call into test database instead of hard-coded test repository.
        // Sql-InMemory Mode
        // Here's an example if you want to use a true snapshot in-memory version of your database.
        // Seems to have a dependency on db migrations which could be a blocker....
        /// <summary>
        /// Tests if calculated bonus matches suggested bonus
        /// </summary>
        /// <returns></returns>
        /// <exception cref="XunitException"></exception>
        [Fact]
        public async Task AttendCourseAsync_CourseAttended_SuggestedBonusMustBeCorrectlyCalculated()
        {
            // arrange
            var connection = new SqliteConnection("Data Source=:memory:");

            // important step
            connection.Open();

            // build an options builder based of the current dbContext
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>()
                                        .UseSqlite(connection);

            var dbContext = new EmployeeDbContext(optionsBuilder.Options);

            // seems to have a dependency on db migrations of EF.
            // That could be a blocker.
            dbContext.Database.Migrate();

            var employeeManagementDataRepository = new EmployeeManagementRepository(dbContext);
            var employeeService = new EmployeeService(employeeManagementDataRepository, new EmployeeFactory());

            // get course from database - "Dealing with Customers 101"
            var courseToAttend = await employeeManagementDataRepository
                .GetCourseAsync(Guid.Parse("844e14ce-c055-49e9-9610-855669c9859b"));

            // get existing employee - "Megan Jones"
            var internalEmployee = await employeeManagementDataRepository
                .GetInternalEmployeeAsync(Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"));

            if (courseToAttend == null || internalEmployee == null)
            {
                throw new XunitException("Arranging the test failed");
            }

            // expected suggested bonus after attending the course
            var expectedSuggestedBonus = internalEmployee.YearsInService
                * (internalEmployee.AttendedCourses.Count + 1) * 100;


            // act
            await employeeService.AttendCourseAsync(internalEmployee, courseToAttend);

            // assert
            Assert.Equal(expectedSuggestedBonus, internalEmployee.SuggestedBonus);






            //// Arrange
            //var connection = new SqliteConnection("Data Source=:memory:");
            //connection.Open();

            //var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>()
            //          .UseSqlite(connection);

            //var dbContext = new EmployeeDbContext(optionsBuilder.Options);
            //dbContext.Database.Migrate();

            //var employeeManagementDataRepository =
            //   new EmployeeManagementRepository(dbContext);

            //var employeeService = new EmployeeService(
            //    employeeManagementDataRepository,
            //    new EmployeeFactory());

            //// get course from database - "Dealing with Customers 101"
            //var courseToAttend = await employeeManagementDataRepository
            //    .GetCourseAsync(Guid.Parse("844e14ce-c055-49e9-9610-855669c9859b"));

            //// get existing employee - "Megan Jones"
            //var internalEmployee = await employeeManagementDataRepository
            //    .GetInternalEmployeeAsync(Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb"));

            //if (courseToAttend == null || internalEmployee == null)
            //{
            //    throw new XunitException("Arranging the test failed");
            //}

            //// expected suggested bonus after attending the course
            //var expectedSuggestedBonus = internalEmployee.YearsInService
            //    * (internalEmployee.AttendedCourses.Count + 1) * 100;

            //// Act
            //await employeeService.AttendCourseAsync(internalEmployee, courseToAttend);

            //// Assert
            //Assert.Equal(expectedSuggestedBonus, internalEmployee.SuggestedBonus);

        }

        [Fact]
        public async Task PromoteInternalEmployeeAsync_IsEligible_JobLevelMustBeIncreased() 
        {
            // Arrange
            var httpClient = new HttpClient(new TestablePromotionEligibilityHandler(true));
            var internalEmployee = new InternalEmployee("Brooklyn", "Cannon", 5, 3000, false, 1);
            var promotionService = new PromotionService(httpClient, new EmployeeManagementTestDataRepository());

            // Act
            await promotionService.PromoteInternalEmployeeAsync(internalEmployee);

            // Assert
            Assert.Equal(2, internalEmployee.JobLevel);
        }
    }
}
