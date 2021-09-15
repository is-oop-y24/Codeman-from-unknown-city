using Isu.Tools;

namespace Isu.Entities
{
    public readonly struct CourseNumber
    {
        public const ushort Max = 9;

        public readonly ushort Val;

        public CourseNumber(ushort val)
        {
            if (val > Max)
            {
                throw new IsuException($"Course number must not be bigger than {Max}");
            }

            Val = val;
        }

        public CourseNumber(char val)
        {
            if (!(val >= '0' && val <= '9'))
            {
                throw new IsuException("Course number must be a number");
            }

            Val = (ushort)(val - '0');
        }
    }
}