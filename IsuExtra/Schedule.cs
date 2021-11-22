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

        private bool LessonOverlapsWithOthers(Lesson newLesson)
        {
            List<Lesson> matches = _lessons.FindAll(lesson =>
            {
                if (lesson.Date.DayOfWeek != newLesson.Date.DayOfWeek || lesson.Auditory != newLesson.Auditory)
                    return false;
                double timeBetweenLessons = (Math.Abs(lesson.Date.Minute - newLesson.Date.Minute) * 0.01) +
                                            Math.Abs(lesson.Date.Hour - newLesson.Date.Hour);
                return timeBetweenLessons < 1.30;
            });
            return matches.Count > 0;
        }
    }
}