using System.Text.RegularExpressions;

using Isu.Tools;

namespace Isu.Entities
{
    public readonly struct GroupName
    {
        public readonly string Val;
        private const string Pattern = @"^M3\d{3}$";

        public GroupName(string name)
        {
            if (!Regex.IsMatch(name, Pattern))
            {
                throw new IsuException($"'{name}' is invalid group name (pattern: M3XXX, X - number)");
            }

            Val = name;
        }
    }
}