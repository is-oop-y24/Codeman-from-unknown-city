using System.Collections.Generic;
using Isu.Entities;

namespace IsuExtra
{
    public class GroupName : Isu.Entities.GroupName
    {
        private static readonly Dictionary<Faculty, string> Prefixes = new ();
        private static bool _prefixesIsInstalled = false;

        public GroupName(Faculty faculty, CourseNumber course, uint groupNumber)
            : base(FacultyToPrefix(faculty), course, groupNumber)
        {
            Faculty = faculty;
        }

        public Faculty Faculty { get; }
        private static string FacultyToPrefix(Faculty faculty)
        {
            if (!_prefixesIsInstalled)
            {
               InstallPrefixes();
               _prefixesIsInstalled = true;
            }

            return Prefixes[faculty];
        }

        private static void InstallPrefixes()
        {
            // Не нашёл точных названий, поэтому выдумал. Думаю, это не так важно
            Prefixes.Add(Faculty.Itip, "M3");
            Prefixes.Add(Faculty.Btins, "B3");
            Prefixes.Add(Faculty.Ctu, "K3");
            Prefixes.Add(Faculty.Fotonika, "F3");
            Prefixes.Add(Faculty.Ftmi, "T3");
        }
    }
}