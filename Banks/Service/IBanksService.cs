using System.Collections.Generic;
using Banks.Accounts;

namespace Banks.Service
{
    public interface IBanksService
    {
        public IBank FindBank(string name);

        public IBank AddBank(
            string name,
            Dictionary<AccountType, object> interestedRates,
            Dictionary<AccountType, double> commissions,
            out string errDesc);
    }
}