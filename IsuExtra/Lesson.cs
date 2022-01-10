using System;

namespace IsuExtra
{
    public readonly struct Lesson
    {
        public readonly string TeacherName;
        public readonly string Auditory;
        public readonly DateTime Date;

        public Lesson(string teacherName, string auditory, DateTime date)
        {
            if (string.IsNullOrEmpty(teacherName) || string.IsNullOrEmpty(auditory))
                throw new ArgumentException("Lesson teacher name or auditory shouldn't be null or empty");
            TeacherName = teacherName;
            Auditory = auditory;
            Date = date;
        }
    }
}