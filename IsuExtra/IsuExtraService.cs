using System.Collections.Generic;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;

namespace IsuExtra
{
    public class IsuExtraService
    {
        private IsuService _isuService = new IsuService();
        private List<OgnpGroup> _ognpGroups = new List<OgnpGroup>();
        private List<Student> _students = new ();

        public OgnpGroup AddOgnp(GroupName name, short maxNStudents)
        {
            OgnpGroup ognpGroup = new (name, maxNStudents);
            if (_ognpGroups.Contains(ognpGroup))
                throw new IsuException($"OGNP {name} already exists");
            _ognpGroups.Add(ognpGroup);
            return ognpGroup;
        }

        public Group AddGroup(GroupName groupName, short maxNStudents) => _isuService.AddGroup(groupName, maxNStudents);

        public Student AddStudent(Faculty faculty, Group group, string name)
        {
            Isu.Entities.Student student = _isuService.AddStudent(group, name);
            Student extraStudent = new (faculty, student);
            _students.Add(extraStudent);
            return extraStudent;
        }

        public void ChangeStudentGroup(Student student, Group newGroup) =>
            _isuService.ChangeStudentGroup(student, newGroup);
        public Group FindGroup(GroupName groupName) => _isuService.FindGroup(groupName);
        public Student FindStudent(string name) => FindExtra(_isuService.FindStudent(name));
        public Student FindStudent(int id) => FindExtra(_isuService.FindStudent(id));
        public Student GetStudent(int id) => FindExtra(_isuService.GetStudent(id));
        public List<Student> FindStudents(GroupName groupName) => FindExtra(_isuService.FindStudents(groupName));

        public List<Student> FindStudents(CourseNumber courseNumber) =>
            FindExtra(_isuService.FindStudents(courseNumber));

        public void AddStudentToOgnp(Student student, OgnpGroup ognpGroup)
        {
            if (student.Faculty == ognpGroup.Name.Faculty)
                throw new IsuException("Can't add student to ognp of same faculty");
            ognpGroup.Add(student);
        }

        public void ChangeStudentOgnp(Student student, OgnpGroup newOgnp)
        {
            OgnpGroup prevOgnp = FindOgnp(student);
            prevOgnp.Remove(student);
            newOgnp.Add(student);
        }

        private Student FindExtra(Isu.Entities.Student oldStudent) =>
            _students.Find(student => student.Id == oldStudent.Id);

        private List<Student> FindExtra(List<Isu.Entities.Student> oldStudents)
        {
            List<Student> extraStudents = new ();
            oldStudents.ForEach(student => extraStudents.Add(FindExtra(student)));
            return extraStudents;
        }

        private OgnpGroup FindOgnp(Student student) => _ognpGroups.Find(ognp => ognp.Contains(student));
    }
}