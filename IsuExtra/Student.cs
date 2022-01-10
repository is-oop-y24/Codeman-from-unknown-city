namespace IsuExtra
{
    public class Student : Isu.Entities.Student
    {
        public Student(Faculty faculty, string name, int id)
            : base(name, id)
        {
            Faculty = faculty;
        }

        public Student(Faculty faculty, Isu.Entities.Student other)
            : this(faculty, other.Name, other.Id)
        { }

        public Faculty Faculty { get; }
    }
}