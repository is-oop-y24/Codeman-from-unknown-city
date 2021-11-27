using System;
using System.Collections.Generic;
using Isu.Tools;

namespace IsuExtra
{
    public class Schedule
    {
        private readonly List<Lesson> _lessons = new ();
        public List<Lesson> Lessons => new List<Lesson>(_lessons);

        public void Add(Lesson lesson)
        {
            if (LessonOverlapsWithOthers(lesson))
                throw new IsuException("The lesson overlaps with others");
            if (_lessons.Contains(lesson))
                throw new IsuException("The lesson already in schedule");
            _lessons.Add(lesson);
        }

        public void Remove(Lesson lesson) => _lessons.Remove(lesson);

        private bool LessonOverlapsWithOthers(Lesson newLesson) =>
            _lessons
                .FindAll(lesson =>
                {
                    if (lesson.Date.DayOfWeek != newLesson.Date.DayOfWeek || lesson.Auditory != newLesson.Auditory)
                        return false;

                    if (newLesson.Date.Hour > lesson.Date.Hour)
                    {
                        DateTime nextLessonMinStartTime = lesson.Date.AddHours(1).AddMinutes(31);
                        return nextLessonMinStartTime.CompareTo(newLesson.Date) > 0;
                    }

                    if (newLesson.Date.Hour < lesson.Date.Hour)
                    {
                        DateTime prevLessonMaxStartTime = lesson.Date.AddHours(-1).AddMinutes(-31);
                        return prevLessonMaxStartTime.CompareTo(newLesson.Date) < 0;
                    }

                    return true;
                })
                .Count > 0;
    }
}