namespace EmployeeManagement.Test.Fixtures
{
    // Collection Fixture Approach - acts as a wrapper around the EmployeeServiceFixture.
    // Can be shared around multiple classes.

    [CollectionDefinition("EmployeeServiceCollection")]
    public class EmployeeServiceCollectionFixture : ICollectionFixture<EmployeeServiceFixture>
    {
        public EmployeeServiceCollectionFixture()
        {
        }
    }
}
