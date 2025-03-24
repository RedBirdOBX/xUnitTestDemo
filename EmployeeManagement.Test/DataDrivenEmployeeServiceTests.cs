using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Test.Fixtures;

namespace EmployeeManagement.Test
{
    // expand...

    [Collection("EmployeeServiceCollection")] // Collection Fixture Approach
    public class DataDrivenEmployeeServiceTests //: IClassFixture<EmployeeServiceFixture>
    {
        private readonly EmployeeServiceFixture _employeeServiceFixture;

        public DataDrivenEmployeeServiceTests(EmployeeServiceFixture employeeServiceFixture)
        {
            _employeeServiceFixture = employeeServiceFixture;
        }

        [Fact]
        public async Task GiveRaise_MinimumRaiseGiven_EmployeeMinimumRaiseGivenMustBeTrue()
        {
            var internalEmployee = new InternalEmployee( "Brooklyn", "Cannon", 5, 3000, false, 1);

            await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, 100);

            Assert.True(internalEmployee.MinimumRaiseGiven);
        }

        [Fact]
        public async Task GiveRaise_MoreThanMinimumRaiseGiven_EmployeeMinimumRaiseGivenMustBeFalse()
        {
            var internalEmployee = new InternalEmployee("Brooklyn", "Cannon", 5, 3000, false, 1);
            await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, 200);
            Assert.False(internalEmployee.MinimumRaiseGiven);
        }



        // example of MEMBER DATA.  This can be placed in another class if needed.
        // property
        public static IEnumerable<object[]> ExampleTestDataForGivenRaise_WithProperty
        {
            get
            {
                return new List<object[]>
                {
                    new object[]{ 100, true},
                    new object[]{ 200, false},
                };
            }
        }

        [Theory]
        [MemberData(nameof(ExampleTestDataForGivenRaise_WithProperty))]
        public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue(int raiseGiven, bool expectedValueForMinRaiseGiven)
        {
            var internalEmployee = new InternalEmployee("Brooklyn", "Cannon", 5, 3000, false, 1);

            await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, raiseGiven);

            Assert.Equal(expectedValueForMinRaiseGiven, internalEmployee.MinimumRaiseGiven);
        }

        [Theory]
        [InlineData("37e03ca7-c730-4351-834c-b66f280cdb01")]
        [InlineData("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e")]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedSecondObligatoryCourse(Guid courseId)
        {
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == courseId );
        }

    }
}
