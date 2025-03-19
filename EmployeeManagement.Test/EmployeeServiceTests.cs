using EmployeeManagement.Business;
using EmployeeManagement.Business.EventArguments;
using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Services.Test;

namespace EmployeeManagement.Test
{
    public class EmployeeServiceTests
    {
        private Guid _firstCourseId = Guid.Parse("37e03ca7-c730-4351-834c-b66f280cdb01");
        private Guid _secondCourseId = Guid.Parse("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e");

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedFirstObligatoryCourse_WithObject()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var obligatoryCourse = employeeRepo.GetCourse(_firstCourseId);
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

            Assert.Contains(obligatoryCourse, internalEmployee.AttendedCourses);
        }

        // slightly cleaner approach
        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedFirstObligatoryCourse_WithPredicate()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == _firstCourseId);
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedSecondObligatoryCourse_WithPredicate()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == _secondCourseId );
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_AttendedCoursesMatchObligatoryCourses()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var obligatoryCourses = employeeRepo.GetCourses(_firstCourseId, _secondCourseId);
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Equal(obligatoryCourses, internalEmployee.AttendedCourses);
        }

        [Fact]
        public void CreateInternalEmployee_InternalEmployeeCreated_AttendedCoursesMustNotBeNew()
        {
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var internalEmployee = employeeService.CreateInternalEmployee("Megan", "Jones");

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
            var employeeRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(employeeRepo, new EmployeeFactory());
            var obligatoryCourses = await employeeRepo.GetCoursesAsync(_firstCourseId, _secondCourseId);
            var internalEmployee = await employeeService.CreateInternalEmployeeAsync("Megan", "Jones");

            // list to search thru , predicate
            Assert.Equal(obligatoryCourses, internalEmployee.AttendedCourses);
        }

        // EXCEPTION TESTING //
        [Fact]
        public async Task GiveRaise_RaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown_Async()
        {
            var employeeService = new EmployeeService(new EmployeeManagementTestDataRepository(), new EmployeeFactory());
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            await Assert.ThrowsAsync<EmployeeInvalidRaiseException>(async () =>
                await employeeService.GiveRaiseAsync(internalEmployee, 50));
        }

        // common mistake - not awaiting gives false positive
        [Fact]
        public void GiveRaise_RaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown_Mistake()
        {
            var employeeService = new EmployeeService(new EmployeeManagementTestDataRepository(), new EmployeeFactory());
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            Assert.ThrowsAsync<EmployeeInvalidRaiseException>(async () =>
                await employeeService.GiveRaiseAsync(internalEmployee, 50));
        }

        // EVENT TESTING //
        [Fact]
        public void NotifyOfAbsence_EmployeeIsAbsent_OnEmployeeIsAbsentMustBeTriggered() 
        { 
            var employeeService = new EmployeeService(new EmployeeManagementTestDataRepository(), new EmployeeFactory());
            var internalEmployee = new InternalEmployee("Ricky", "Bobby", 5, 3000, false, 1);

            // accept 3 functions:
            // 1 - to attach an event handler
            // 2 - to detach event handler
            // 3 - code that actually raises the event
            Assert.Raises<EmployeeIsAbsentEventArgs>(
                handler => employeeService.EmployeeIsAbsent += handler, 
                handler => employeeService.EmployeeIsAbsent -= handler,
                () => employeeService.NotifyOfAbsence(internalEmployee));
        }
    }
}
