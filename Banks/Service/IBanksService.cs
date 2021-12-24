using System.Collections.Generic;
using Banks.Accounts;

namespace Banks.Service
{
    public interface IBanksService
    {
        IBank FindBank(string name);

        IBank AddBank(
            string name,
            Dictionary<AccountType, object> interestedRates,
            Dictionary<AccountType, double> commissions,
            out string errDesc);
    }
}