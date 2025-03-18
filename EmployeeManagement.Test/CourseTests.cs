using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
    public class CourseTests
    {
        [Fact]
        public void CourseConstructor_ConstructCourse_IsNewMustBeSetToTrue()
        {
            // Act
            var course = new Course("Test Course");

            // Assert
            Assert.True(course.IsNew);
        }

    }
}
