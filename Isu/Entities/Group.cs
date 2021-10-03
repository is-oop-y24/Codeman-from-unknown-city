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

        private Group(Group other, Student student, ActionWithStudent action)
            : this(other.Name, other._maxNStudents)
        {
            _nStudents = other._nStudents;

            foreach ((int id, Student otherStudent) in other._students)
            {
                _students[id] = otherStudent;
            }

            switch (action)
            {
                case ActionWithStudent.Delete:
                    _students.Remove(student.Id);
                    _nStudents--;
                    break;

                case ActionWithStudent.Add:
                    _students[student.Id] = student;
                    _nStudents++;

                    if (_nStudents > _maxNStudents)
                    {
                        throw new IsuException("Reached max number of students per group");
                    }

                    break;
            }
        }

        private enum ActionWithStudent
        {
            Delete,
            Add,
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

        public Group Add(Student student) => new Group(this, student, ActionWithStudent.Add);

        public Group Remove(Student student) => new Group(this, student, ActionWithStudent.Delete);

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
    }
}