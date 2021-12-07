using System.Collections.Generic;
using Banks.Accounts;

namespace Banks.Service
{
    public interface IBanksService
    {
        public IBank FindBank(string name);

        public IBank AddBank(
            string name,
            Dictionary<Account.AccountType, object> interestedRates,
            Dictionary<Account.AccountType, double> commissions,
            out string errDesc);
    }
}