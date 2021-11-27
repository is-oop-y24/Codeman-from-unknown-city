using System;
using NUnit.Framework;

using Isu;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Tests
{
    public class IsuExtraTest
    {
        private const short MaxNStudents = 30;
        private IsuExtraService _isuService; 
        [SetUp]
        public void Setup()
        {
            _isuService = new IsuExtraService();
        }

        [Test]
        public void ChangeOgnp_OgnpChanged()
        {
            Group group = _isuService.AddGroup(new GroupName(Faculty.Itip, CourseNumber.First, 09), MaxNStudents);
            Student student = _isuService.AddStudent(Faculty.Itip, group, "Alex");
            OgnpGroup prevOgnpGroup = _isuService.AddOgnp(new GroupName(Faculty.Ctu, CourseNumber.First, 03), MaxNStudents);
            OgnpGroup nextOgnpGroup = _isuService.AddOgnp(new GroupName(Faculty.Ctu, CourseNumber.First, 05), MaxNStudents);
            _isuService.AddStudentToOgnp(student, prevOgnpGroup);
            _isuService.ChangeStudentOgnp(student, nextOgnpGroup);
            Assert.True(!prevOgnpGroup.Contains(student) && nextOgnpGroup.Contains(student));
        }

        [Test]
        public void AddStudentToOgnp_StudentAdded()
        {
            Group group = _isuService.AddGroup(new GroupName(Faculty.Itip, CourseNumber.First, 09), MaxNStudents);
            Student student = _isuService.AddStudent(Faculty.Itip, group, "Alex");
            OgnpGroup ognp = _isuService.AddOgnp(new GroupName(Faculty.Ctu, CourseNumber.First, 03), MaxNStudents);
            _isuService.AddStudentToOgnp(student, ognp);
            Assert.True(ognp.Contains(student));
        }

        [Test]
        public void CrossSchedule_ThrowException()
        {
            OgnpGroup ognp = _isuService.AddOgnp(new GroupName(Faculty.Ctu, CourseNumber.First, 03), MaxNStudents);
            ognp.Add(new Lesson("Tech", "349A", DateTime.Now));
            Assert.Catch<IsuException>(()=> ognp.Add(new Lesson("Tech", "349A", DateTime.Now.AddHours(1))));
        }
    }
}