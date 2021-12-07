using System;

namespace Banks.Tools
{
    public class WaybackMachine
    {
        private static WaybackMachine _instance;
        private WaybackMachine()
        { }

        public event EventHandler NewDay;
        public static WaybackMachine Instance => _instance ??= new WaybackMachine();

        public void RewindTimeForwardOnDay(EventArgs eventArgs = null) => RewindTimeOn(1);
        public void RewindTimeForwardOnMount(EventArgs eventArgs = null) => RewindTimeOn(30);
        public void RewindTimeForwardOnYear(EventArgs eventArgs) => RewindTimeOn(365);

        public void RewindTimeOn(uint nDays)
        {
            for (int i = 0; i < nDays; i++)
            {
                NewDay?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}