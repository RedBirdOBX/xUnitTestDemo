using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Services.Test;

namespace EmployeeManagement.Test.Fixtures
{
    // Class Fixture Approach //
    // Build this per class. //
    // https://app.pluralsight.com/ilx/video-courses/d1a07995-8bbd-4124-a48f-b1f7f672091e/bf7b9b5f-fe0f-4ea0-be80-4a7b2ecbf660/0484ebd6-a8ea-4043-ad9b-0df159976339

    public class EmployeeServiceFixture : IDisposable
    {
        // Build out the dependencies need for our Test Class.
        // Make read-only. Tests can get but not set these.
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
