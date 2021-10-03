using System.Collections.Generic;

using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private readonly Dictionary<string, Group>[] _groupsByCourse = new Dictionary<string, Group>[(int)CourseNumber.Fourth + 1];
        private int _newStudentId = 1;

        public IsuService()
        {
            for (int i = 0; i < _groupsByCourse.Length; i++)
            {
                _groupsByCourse[i] = new Dictionary<string, Group>();
            }
        }

        public Group AddGroup(GroupName groupName, short maxNStudents)
        {
            if (FindGroup(groupName) != null)
            {
                throw new IsuException($"Group {groupName} already exist");
            }

            var group = new Group(groupName, maxNStudents);
            _groupsByCourse[groupName.Course][groupName.ToString()] = group;
            return group;
        }

        public Student AddStudent(Group group, string studentName)
        {
            var student = new Student(studentName, _newStudentId++);

            try
            {
                _groupsByCourse[group.Name.Course][group.Name.ToString()] = FindGroup(group.Name).Add(student);
            }
            catch (KeyNotFoundException)
            {
                throw new IsuException($"Group {group.Name} is not found in isu service");
            }

            return student;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            Group oldGroup = FindGroup(student.Id) ?? throw new IsuException($"Student {student.Name} is not in any group");
            try
            {
                _groupsByCourse[oldGroup.Name.Course][oldGroup.Name.ToString()] = oldGroup.Remove(student);
                student = new Student(student);
                _groupsByCourse[newGroup.Name.Course][newGroup.Name.ToString()] = FindGroup(newGroup.Name).Add(student);
            }
            catch (KeyNotFoundException e)
            {
                throw new IsuException(e.Message);
            }
        }

        public Group FindGroup(GroupName groupName)
        {
            try
            {
                return _groupsByCourse[groupName.Course][groupName.ToString()];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            var groups = new List<Group>();

            foreach ((string _, Group group) in _groupsByCourse[(int)courseNumber])
            {
                groups.Add(group);
            }

            return groups;
        }

        public Student FindStudent(string studentName)
        {
            for (int i = 1; i < _groupsByCourse.Length; i++)
            {
                foreach ((string _, Group group) in _groupsByCourse[i])
                {
                    Student match = group.Find(studentName);
                    if (match != null)
                        return match;
                }
            }

            return null;
        }

        public Student FindStudent(int id)
        {
            for (int i = 1; i < _groupsByCourse.Length; i++)
            {
                foreach ((string _, Group group) in _groupsByCourse[i])
                {
                    Student match = group.Find(id);
                    if (match != null)
                        return match;
                }
            }

            return null;
        }

        public Student GetStudent(int id) => FindStudent(id) ?? throw new IsuException($"Cannot find student with id {id}");

        public List<Student> FindStudents(GroupName groupName) => FindGroup(groupName)?.Students ?? new List<Student>();

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var students = new List<Student>();
            FindGroups(courseNumber).ForEach(group => students.AddRange(group.Students));
            return students;
        }

        private Group FindGroup(int studentId)
        {
            foreach (Dictionary<string, Group> groups in _groupsByCourse)
            {
                foreach ((string _, Group group) in groups)
                {
                    if (group.Contains(studentId))
                    {
                        return group;
                    }
                }
            }

            return null;
        }
    }
}