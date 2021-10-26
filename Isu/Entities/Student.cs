using Isu.Tools;

namespace Isu.Entities
{
    public class Student
    {
        public Student(string name, int id)
        {
            Name = Validate(name);
            Id = id;
        }

        public Student(Student other)
            : this(other.Name, other.Id)
        { }

        public string Name { get; }
        public int Id { get; }
        public static string Validate(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new IsuException("Student name should not be null or empty");
            }

            return name;
        }
    }
}