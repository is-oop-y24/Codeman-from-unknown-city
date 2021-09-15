using System;
using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private const short MaxNStudent = 35;

        private readonly List<Student> _students = new List<Student>();
        private short _nStudent = 0;

        public Group(GroupName name)
        {
            Name = name;
        }

        public GroupName Name { get; }

        public List<Student> Students => new List<Student>(_students);

        public void Add(Student student)
        {
            if (_nStudent++ == MaxNStudent)
            {
                throw new IsuException("Reached max number of students per group");
            }

            _students.Add(student);
        }

        public Student Find(Predicate<Student> predicate) => _students.Find(predicate);

        public void Remove(Student student)
        {
            if (_students.Remove(student))
                _nStudent--;
        }
    }
}