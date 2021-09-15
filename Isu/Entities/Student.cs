using Isu.Tools;

namespace Isu.Entities
{
    public struct Student
    {
        public readonly string Name;
        public readonly int Id;
        public Group Group;

        public Student(Group group, string name, int id)
        {
            Group = group;
            Name = Validate(name);
            Id = id;
        }

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