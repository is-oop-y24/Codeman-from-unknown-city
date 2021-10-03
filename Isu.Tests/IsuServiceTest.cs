using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;
        private const short MaxNStudents = 3;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService();
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Group testGroup = _isuService.AddGroup(new GroupName("M3", CourseNumber.Second, 9), MaxNStudents);
            Student testStudent = _isuService.AddStudent(testGroup, "Name");
            Assert.True(_isuService.FindGroup(testGroup.Name).Contains(testStudent));
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Group testGroup = _isuService.AddGroup(new GroupName("M3", CourseNumber.Second, 9), MaxNStudents);
            Assert.Catch<IsuException>(() =>
            {
                for (int i = 0; i <= MaxNStudents; i++)
                {
                    _isuService.AddStudent(testGroup, "Name");
                }
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            Assert.Catch<IsuException>(() =>
            {
                new GroupName("P3", CourseNumber.Second, 09); // invalid prefix
            });
            Assert.Catch<IsuException>(() =>
            {
                new GroupName("M3", CourseNumber.Second, 1231); // invalid group number
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            Group testGroup1 = _isuService.AddGroup(new GroupName("M3", CourseNumber.First, 9), MaxNStudents);
            Group testGroup2 = _isuService.AddGroup(new GroupName("M3", CourseNumber.Second, 9), MaxNStudents);
            Student testStudent = _isuService.AddStudent(testGroup1, "Name");
            _isuService.ChangeStudentGroup(testStudent, testGroup2);
            Assert.True(!_isuService.FindGroup(testGroup1.Name).Contains(testStudent) && 
                        _isuService.FindGroup(testGroup2.Name).Contains(testStudent));
        }
    }
}