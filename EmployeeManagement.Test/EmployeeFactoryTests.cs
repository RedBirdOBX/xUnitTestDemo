using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTests
    {
        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryMustBe2500()
        {
            // Arrange
            var factory = new EmployeeFactory();

            // Act
            var employee = (InternalEmployee)factory.CreateEmployee("John", "Doe");

            // Assert
            Assert.Equal(2500, employee.Salary);
        }
    }
}
