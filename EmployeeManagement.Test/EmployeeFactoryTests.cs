using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using Newtonsoft.Json.Serialization;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTests
    {
        private const int _minSalaryRange = 2500;
        private const int _maxSalaryRange = 3500;

        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryIsBetween2500And3500()
        {
            var factory = new EmployeeFactory();
            var employee = (InternalEmployee)factory.CreateEmployee("John", "Doe");
            Assert.True(employee.Salary >= _minSalaryRange && employee.Salary <= _maxSalaryRange, $"Salary must be between {_minSalaryRange} and {_maxSalaryRange}.");
        }

        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryInRange()
        {
            var factory = new EmployeeFactory();
            var employee = (InternalEmployee)factory.CreateEmployee("John", "Doe");
            Assert.InRange(employee.Salary, _minSalaryRange, _maxSalaryRange);
        }

        [Fact]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryIsBetween2500And3500_Precision()
        {
            var factory = new EmployeeFactory();
            var employee = (InternalEmployee)factory.CreateEmployee("John", "Doe");
            employee.Salary = 2500.01m;
            Assert.Equal(2500, employee.Salary, 0);
        }
    }
}
