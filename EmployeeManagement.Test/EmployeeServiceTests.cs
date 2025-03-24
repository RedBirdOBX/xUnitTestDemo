using EmployeeManagement.Business;
using EmployeeManagement.Business.EventArguments;
using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Services.Test;
using EmployeeManagement.Test.Fixtures;

namespace EmployeeManagement.Test
{
    // IClassFixture is used to share the same instance of the fixture class across all tests in the test class
    // Instantiated before first test is fun and will auto-dispose once last test is run.

    // expand...

    [Collection("EmployeeServiceCollection")] // Collection Fixture Approach
    public class EmployeeServiceTests //: IClassFixture<EmployeeServiceFixture>  commented out for Collection Fixture Approach
    {
        private readonly Guid _firstCourseId = Guid.Parse("37e03ca7-c730-4351-834c-b66f280cdb01");
        private readonly Guid _secondCourseId = Guid.Parse("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e");
        private readonly EmployeeServiceFixture _employeeServiceFixture;

        public EmployeeServiceTests(EmployeeServiceFixture employeeServiceFixture)
        {
            _employeeServiceFixture = employeeServiceFixture;
        }


        /// <summary>
        /// Original test set up.  This version has a set up of the employee repo and employee service.
        /// Building each test like this would be expensive. x seconds per implementation of the test data repo.
        /// </summary>
        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedFirstObligatoryCourse_WithObject_Orig()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var obligatoryCourse = employeeRepo.GetCourse(_firstCourseId);
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

            Assert.Contains(obligatoryCourse, internalEmployee.AttendedCourses);
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedFirstObligatoryCourse_WithObject()
        {
            var obligatoryCourse = _employeeServiceFixture.EmployeeManagementTestDataRepository.GetCourse(_firstCourseId);
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");
            Assert.Contains(obligatoryCourse, internalEmployee.AttendedCourses);
        }

        // slightly cleaner approach
        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedFirstObligatoryCourse_WithPredicate()
        {
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == _firstCourseId);
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedSecondObligatoryCourse_WithPredicate()
        {
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == _secondCourseId );
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_AttendedCoursesMatchObligatoryCourses()
        {
            var obligatoryCourses = _employeeServiceFixture.EmployeeManagementTestDataRepository.GetCourses(_firstCourseId, _secondCourseId);
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Equal(obligatoryCourses, internalEmployee.AttendedCourses);
        }

        /// <summary>
        /// Asserting on a collection of objects
        /// </summary>
        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_AttendedCoursesMustNotBeNew()
        {
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            foreach (var course in internalEmployee.AttendedCourses)
            {
                Assert.False(course.IsNew);
            }

            // or
            Assert.All(internalEmployee.AttendedCourses, (course) => Assert.False(course.IsNew));
        }

        // testing with async
        [Fact]
        public async Task CreateInternalEmployee_InternalEmployeeCreated_AttendedCoursesMatchObligatoryCourses_Async()
        {
            var obligatoryCourses = await _employeeServiceFixture.EmployeeManagementTestDataRepository.GetCoursesAsync(_firstCourseId, _secondCourseId);
            var internalEmployee = await _employeeServiceFixture.EmployeeService.CreateInternalEmployeeAsync("Megan", "Jones");

            // list to search thru , predicate
            Assert.Equal(obligatoryCourses, internalEmployee.AttendedCourses);
        }

        // EXCEPTION TESTING //
        [Fact]
        public async Task GiveRaise_RaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown_Async()
        {
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            await Assert.ThrowsAsync<EmployeeInvalidRaiseException>(async () =>
                await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, 50));
        }

        // common mistake - not awaiting gives false positive
        [Fact]
        public void GiveRaise_RaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown_Mistake()
        {
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            Assert.ThrowsAsync<EmployeeInvalidRaiseException>(async () =>
                await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, 50));
        }

        // EVENT TESTING //
        [Fact]
        public void NotifyOfAbsence_EmployeeIsAbsent_OnEmployeeIsAbsentMustBeTriggered()
        {
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            // accept 3 functions:
            // 1 - to attach an event handler
            // 2 - to detach event handler
            // 3 - code that actually raises the event
            Assert.Raises<EmployeeIsAbsentEventArgs>(
                handler => _employeeServiceFixture.EmployeeService.EmployeeIsAbsent += handler,
                handler => _employeeServiceFixture.EmployeeService.EmployeeIsAbsent -= handler,
                () => _employeeServiceFixture.EmployeeService.NotifyOfAbsence(internalEmployee));
        }
    }
}
