using System.Collections.Generic;
using Isu.Entities;

namespace IsuExtra
{
    public class OgnpGroup
    {
        private readonly Schedule _schedule = new ();
        private Group _group;

        public OgnpGroup(GroupName name, short maxNStudents)
        {
            _group = new Group(name, maxNStudents);
            Name = name;
        }

        public GroupName Name { get; }
        public List<Isu.Entities.Student> Students => _group.Students;
        public List<Lesson> Lessons => _schedule.Lessons;

        public void Add(Lesson lesson) => _schedule.Add(lesson);
        public void Remove(Lesson lesson) => _schedule.Add(lesson);
        public void Add(Student student) => _group = _group.Add(student);
        public void Remove(Student student) => _group = _group.Remove(student);
        public bool Contains(int studentId) => _group.Contains(studentId);
        public bool Contains(Student student) => _group.Contains(student.Id);
        public Isu.Entities.Student Find(int id) => _group.Find(id);
        public Isu.Entities.Student Find(string name) => _group.Find(name);
    }
}