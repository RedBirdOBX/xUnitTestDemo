using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTests : IDisposable
    {
        private const int _minSalaryRange = 2500;
        private const int _maxSalaryRange = 3500;
        private EmployeeFactory _employeeFactory;

        // Good example of Constructor and Dispose Approach. //
        // We run thru the set up code and clean code for EACH test. //

        public EmployeeFactoryTests()
        {
            _employeeFactory = new EmployeeFactory();
        }

        public void Dispose()
        {
            // use to clean up any state/data that is left over
        }


        [Fact]
        [Trait("Category","EmployeeFactory_CreateEmployee_Salary")]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryIsBetween2500And3500()
        {
            var employee = (InternalEmployee)_employeeFactory.CreateEmployee("John", "Doe");
            Assert.True(employee.Salary >= _minSalaryRange && employee.Salary <= _maxSalaryRange, $"Salary must be between {_minSalaryRange} and {_maxSalaryRange}.");
        }

        [Fact]
        [Trait("Category","EmployeeFactory_CreateEmployee_Salary")]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryInRange()
        {
            var employee = (InternalEmployee)_employeeFactory.CreateEmployee("John", "Doe");
            Assert.InRange(employee.Salary, _minSalaryRange, _maxSalaryRange);
        }

        [Fact]
        [Trait("Category","EmployeeFactory_CreateEmployee_Salary")]
        public void CreateEmployee_ConstructInternalEmployee_DefaultSalaryIsBetween2500And3500_Precision()
        {
            var employee = (InternalEmployee)_employeeFactory.CreateEmployee("John", "Doe");
            employee.Salary = 2500.01m;
            Assert.Equal(2500, employee.Salary, 0);
        }

        // OBJECT TYPES //
        [Fact]
        [Trait("Category","EmployeeFactory_CreateEmployee_ReturnType")]
        public void CreateEmployee_IsExternalIsTrue_ReturnTypeMustBeExternalEmployee()
        {
            var employee = _employeeFactory.CreateEmployee("Ricky", "Bobby", "ACME", true);
            Assert.IsType<ExternalEmployee>(employee);
        }

        [Fact]
        [Trait("Category","EmployeeFactory_CreateEmployee_ReturnType")]
        public void CreateEmployee_IsExternalIsTrue_ReturnTypeMustBeInternalEmployee()
        {
            var employee = _employeeFactory.CreateEmployee("Ricky", "Bobby", "ACME", false);
            Assert.IsType<InternalEmployee>(employee);
        }
    }
}
