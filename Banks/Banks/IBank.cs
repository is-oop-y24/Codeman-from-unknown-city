using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Clients;
using Banks.InterestedRates;

namespace Banks.Banks
{
    public interface IBank
    {
        string Name { get; }
        Dictionary<AccountType, IInterestedRate> InterestedRates { get; }
        Dictionary<AccountType, double> Commissions { get; }
        Client AddClient(ClientInfo clientInfo);
        void AddAccount(Client client, Account account);
        Client FindClientByPhoneNumber(string num);
        void GetCommission(object sender, EventArgs eventArgs);
        void ChargeInterest(object sender, EventArgs eventArgs);
    }
}