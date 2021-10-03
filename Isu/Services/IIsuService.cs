using System.Collections.Generic;

using Isu.Entities;

namespace Isu.Services
{
    public interface IIsuService
    {
        Group AddGroup(GroupName groupName, short maxNStudents);
        Student AddStudent(Group group, string studentName);
        Student FindStudent(string studentName);
        Student GetStudent(int id);
        List<Student> FindStudents(GroupName groupName);
        List<Student> FindStudents(CourseNumber courseNumber);
        Group FindGroup(GroupName groupName);
        List<Group> FindGroups(CourseNumber courseNumber);

        void ChangeStudentGroup(Student student, Group newGroup);
    }
}