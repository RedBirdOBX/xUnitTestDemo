using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Services.Test;

namespace EmployeeManagement.Test.Fixtures
{
    // Class Fixture Approach
    // Expand on the concept of this.......
    // Somewhat simpler approach. adopt this.

    public class EmployeeServiceFixture : IDisposable
    {
        public IEmployeeManagementRepository EmployeeManagementTestDataRepository { get; }
        
        public EmployeeService EmployeeService { get; }

        public EmployeeServiceFixture()
        {
            EmployeeManagementTestDataRepository = new EmployeeManagementTestDataRepository();
            EmployeeService = new EmployeeService(EmployeeManagementTestDataRepository, new EmployeeFactory());
        }

        public void Dispose()
        {
            // clean up set up code if required
        }
    }
}
