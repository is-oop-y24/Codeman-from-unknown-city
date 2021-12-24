using System.Collections.Generic;
using Banks.Accounts;
using Banks.Banks;

namespace Banks.Service
{
    public class BanksService : IBanksService
    {
        private readonly List<Bank> _banks;
        private readonly CentralBank _centralBank;

        public BanksService(ulong onWhichDayNotifyAboutCommission)
        {
            _banks = new List<Bank>();
            _centralBank = new CentralBank(onWhichDayNotifyAboutCommission);
        }

        public IBank AddBank(
            string name,
            Dictionary<AccountType, object> interestedRates,
            Dictionary<AccountType, double> commissions,
            out string errDesc)
        {
            errDesc = null;

            if (FindBank(name) != null)
            {
                errDesc = $"Bank \"{name}\" is already exists";
                return null;
            }

            var bank = new Bank(name, interestedRates, commissions);
            _centralBank.RegisterBank(bank);
            _banks.Add(bank);
            return bank;
        }

        public IBank FindBank(string name) => _banks.Find(bank => bank.Name == name);
    }
}