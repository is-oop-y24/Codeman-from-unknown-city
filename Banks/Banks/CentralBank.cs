using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Tools;

namespace Banks.Banks
{
    public class CentralBank
    {
        private readonly ulong _onWhichDayNotifyAboutCommission;
        private ulong _daysCounter;

        public CentralBank(ulong onWhichDayNotifyAboutCommission)
        {
            if (onWhichDayNotifyAboutCommission == 0)
                throw new ArgumentException($"{nameof(onWhichDayNotifyAboutCommission)} must be bigger than 0");

            _onWhichDayNotifyAboutCommission = onWhichDayNotifyAboutCommission;
            _daysCounter = 0;
            WaybackMachine.Instance.NewDay += OnNewDay;
        }

        public event EventHandler TimeToGetCommission;

        public void RegisterBank(Bank bank)
        {
            TimeToGetCommission += bank.GetCommission;
            TimeToGetCommission += bank.ChargeInterest;
        }

        private void OnNewDay(object sender, EventArgs eventArgs)
        {
            if (++_daysCounter % _onWhichDayNotifyAboutCommission == 0)
                TimeToGetCommission?.Invoke(this, EventArgs.Empty);
        }
    }
}