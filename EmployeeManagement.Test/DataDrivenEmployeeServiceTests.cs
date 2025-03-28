using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Test.Fixtures;
using EmployeeManagement.Test.TestData;

namespace EmployeeManagement.Test
{
    // Collection Fixture Approach //
    // This class illustrates the use of the ICollectionFixture can be shared among multiple
    // classes: this one and the EmployeeServiceTestsAddtl class.  

    [Collection("EmployeeServiceCollection")]
    public class DataDrivenEmployeeServiceTests
    {
        private readonly EmployeeServiceFixture _employeeServiceFixture;

        // class data
        public static IEnumerable<object[]> ExampleTestDataForGivenRaise_WithProperty
        {
            // this is a static property but could also be a method.

            get
            {
                return new List<object[]>
                {
                    new object[]{ 100, true},
                    new object[]{ 200, false},
                };
            }
        }


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

        // easy
        [Theory]
        [InlineData("37e03ca7-c730-4351-834c-b66f280cdb01")]
        [InlineData("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e")]
        public void CreateInternalEmployee_InternalEmployeeCreated_MustHaveAttendedSecondObligatoryCourse(Guid courseId)
        {
            var internalEmployee = _employeeServiceFixture.EmployeeService.CreateInternalEmployee("Megan", "Jones");

            // list to search thru , predicate
            Assert.Contains(internalEmployee.AttendedCourses, (course) => course.Id == courseId );
        }

        // moderate
        [Theory]
        [MemberData(nameof(ExampleTestDataForGivenRaise_WithProperty))]
        public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue(int raiseGiven, bool expectedValueForMinRaiseGiven)
        {
            var internalEmployee = new InternalEmployee("Brooklyn", "Cannon", 5, 3000, false, 1);
            await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, raiseGiven);
            Assert.Equal(expectedValueForMinRaiseGiven, internalEmployee.MinimumRaiseGiven);
        }

        // more complex
        [Theory]
        [ClassData(typeof(StronglyTypedEmployeeServiceTestData_FromFile))]
        public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue2(int raiseGiven, bool expectedValueForMinRaiseGiven)
        {
            var internalEmployee = new InternalEmployee("Brooklyn", "Cannon", 5, 3000, false, 1);
            await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, raiseGiven);
            Assert.Equal(expectedValueForMinRaiseGiven, internalEmployee.MinimumRaiseGiven);
        }
    }
}
