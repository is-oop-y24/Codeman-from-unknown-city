using System;
using System.Collections.Generic;

using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private readonly List<Group>[] _groupsByCourse = new List<Group>[CourseNumber.Max + 1];
        private int _newStudentId = 1;

        public IsuService()
        {
            for (int i = 0; i < _groupsByCourse.Length; i++)
            {
                _groupsByCourse[i] = new List<Group>();
            }
        }

        public Group AddGroup(GroupName groupName)
        {
            if (FindGroup(groupName) != null)
            {
                throw new IsuException($"Group {groupName.Val} already exist");
            }

            var group = new Group(groupName);
            const int courseNumberIndex = 2;
            var courseNumber = new CourseNumber(groupName.Val[courseNumberIndex]);
            _groupsByCourse[courseNumber.Val].Add(group);
            return group;
        }

        public Student AddStudent(Group group, string studentName)
        {
            var student = new Student(group, studentName, _newStudentId++);
            group.Add(student);
            return student;
        }

        public void ChangeStudentGroup(ref Student student, Group newGroup)
        {
            student.Group.Remove(student);
            student.Group = newGroup;
            newGroup.Add(student);
        }

        public Group FindGroup(GroupName groupName)
        {
            for (ushort i = 0; i <= CourseNumber.Max; i++)
            {
                List<Group> groups = _groupsByCourse[i];
                Group match = groups.Find(group => group.Name.Val == groupName.Val);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        public List<Group> FindGroups(CourseNumber courseNumber) => _groupsByCourse[courseNumber.Val];

        public Student? FindStudent(string studentName) => FindStudent(student => student.Name == studentName);

        public Student GetStudent(int id)
        {
            Student? match = FindStudent(student => student.Id == id);
            if (match == null)
            {
                throw new IsuException($"Cannot find student with id {id}");
            }

            return (Student)match;
        }

        public List<Student> FindStudents(GroupName groupName) => FindGroup(groupName).Students;

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var students = new List<Student>();
            FindGroups(courseNumber).ForEach(group => students.AddRange(group.Students));
            return students;
        }

        private Student? FindStudent(Predicate<Student> predicate)
        {
            for (ushort i = 0; i <= CourseNumber.Max; i++)
            {
                List<Group> groups = _groupsByCourse[i];
                foreach (Group group in groups)
                {
                    Student? match = group.Find(predicate);
                    if (match != null)
                    {
                        return match;
                    }
                }
            }

            return null;
        }
    }
}