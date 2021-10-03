using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class GroupName
    {
        private static readonly List<string> PossiblePrefixes = new List<string> { "M3", };
        private readonly string _stringRepresentation;
        private CourseNumber _course;

        public GroupName(string prefix, CourseNumber course, uint groupNumber)
        {
            if (!PossiblePrefixes.Contains(prefix))
            {
                throw new IsuException($"'{prefix}' is invalid group prefix");
            }

            if (!(groupNumber > 0 && groupNumber < 16))
            {
                throw new IsuException($"Group number should be in range from 1 to 15");
            }

            _course = course;
            _stringRepresentation = $"{prefix}{Course}{(groupNumber < 10 ? "0" : string.Empty)}{groupNumber}";
        }

        public int Course => (int)_course;

        public override string ToString() => _stringRepresentation;
    }
}