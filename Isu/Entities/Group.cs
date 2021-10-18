using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private readonly Dictionary<int, Student> _students = new Dictionary<int, Student>();
        private readonly short _maxNStudents;
        private readonly short _nStudents;

        public Group(GroupName name, short maxNStudents)
        {
            Name = name;
            _maxNStudents = maxNStudents;
            _nStudents = 0;
        }

        private Group(Dictionary<int, Student> students, short maxNStudents, short nStudents, GroupName name)
            : this(name, maxNStudents)
        {
            if (nStudents > maxNStudents)
            {
                throw new IsuException("Reached max number of students per group");
            }

            _students = students;
            _nStudents = nStudents;
        }

        public GroupName Name { get; }
        public List<Student> Students
        {
            get
            {
                var students = new List<Student>();

                foreach ((int _, Student student) in _students)
                {
                    students.Add(student);
                }

                return students;
            }
        }

        public Group Add(Student student) => new Builder(this).Add(student).Build();

        public Group Remove(Student student) => new Builder(this).Remove(student).Build();

        public bool Contains(int studentId) => _students.ContainsKey(studentId);
        public bool Contains(Student student) => Contains(student.Id);

        public Student Find(int id) => _students[id];

        public Student Find(string name)
        {
            foreach ((int _, Student student) in _students)
            {
                if (student.Name == name)
                    return student;
            }

            return null;
        }

        private class Builder
        {
            private readonly Dictionary<int, Student> _students = new Dictionary<int, Student>();
            private readonly short _maxNStudents;
            private readonly GroupName _groupName;
            private short _nStudents;

            public Builder(Group group)
            {
                foreach ((int id, Student student) in group._students)
                {
                    _students[id] = student;
                }

                _maxNStudents = group._maxNStudents;
                _nStudents = group._nStudents;
                _groupName = group.Name;
            }

            public Builder Remove(Student student)
            {
                _students.Remove(student.Id);
                _nStudents--;
                return this;
            }

            public Builder Add(Student student)
            {
                _students[student.Id] = student;
                _nStudents++;
                return this;
            }

            public Group Build() => new Group(_students, _maxNStudents, _nStudents, _groupName);
        }
    }
}