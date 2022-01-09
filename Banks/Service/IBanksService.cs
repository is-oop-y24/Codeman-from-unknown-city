using System.Collections.Generic;
using Banks.Accounts;
using Banks.Banks;
using Banks.InterestedRates;

namespace Banks.Service
{
    public interface IBanksService
    {
        IBank FindBank(string name);

        IBank AddBank(
            string name,
            Dictionary<AccountType, IInterestedRate> interestedRates,
            Dictionary<AccountType, double> commissions,
            out string errDesc);
    }
}