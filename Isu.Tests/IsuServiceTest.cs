using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService();
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Group testGroup = _isuService.AddGroup(new GroupName("M3209"));
            Student testStudent = _isuService.AddStudent(testGroup, "Name");
            Assert.True(testGroup.Students.Contains(testStudent) && testStudent.Group == testGroup);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Group testGroup = _isuService.AddGroup(new GroupName("M3209"));
            Assert.Catch<IsuException>(() =>
            {
                for (int i = 0; i < 10000; i++)
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
                Group testGroup = _isuService.AddGroup(new GroupName("M32O9")); // using letter "O" instead 0
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            Group testGroup1 = _isuService.AddGroup(new GroupName("M3109"));
            Group testGroup2 = _isuService.AddGroup(new GroupName("M3209"));
            Student testStudent = _isuService.AddStudent(testGroup1, "Name");
            _isuService.ChangeStudentGroup(ref testStudent, testGroup2);
            Assert.True(!testGroup1.Students.Contains(testStudent) && 
                        testGroup2.Students.Contains(testStudent)  && 
                        testStudent.Group == testGroup2);
        }
    }
}