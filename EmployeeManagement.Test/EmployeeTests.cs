using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
    public class EmployeeTests
    {
        [Fact]
        public void EmployeeFullNamePropertyGetter_InputFirstNameAndLastName_FullNameIsConcatenation()
        {
            var employee = new InternalEmployee("John", "Doe", 1, 3000, false, 1);
            var fullName = employee.FullName;
            Assert.Equal("John Doe", fullName, ignoreCase: true);
        }

        [Fact]
        public void EmployeeFullNamePropertyGetter_InputFirstNameAndLastName_FullNameStartsWithFullName()
        {
            var employee = new InternalEmployee("John", "Doe", 1, 3000, false, 1);
            Assert.StartsWith(employee.FirstName, employee.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void EmployeeFullNamePropertyGetter_InputFirstNameAndLastName_FullNameEndsWithLastName()
        {
            var employee = new InternalEmployee("John", "Doe", 1, 3000, false, 1);
            Assert.EndsWith(employee.LastName, employee.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void EmployeeFullNamePropertyGetter_InputFirstNameAndLastName_FullNameContactsPartOfConcatenation()
        {
            var employee = new InternalEmployee("John", "Doe", 1, 3000, false, 1);
            Assert.Contains("ohn", employee.FullName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
