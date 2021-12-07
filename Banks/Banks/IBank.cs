using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Clients;

namespace Banks
{
    public interface IBank
    {
        public string Name { get; }
        public Dictionary<Account.AccountType, object> InterestedRates { get; }
        public Dictionary<Account.AccountType, double> Commissions { get; }
        public Client AddClient(ClientInfo clientInfo);
        public void AddAccount(Client client, Account account);
        public Client FindClientByPhoneNumber(string num);
        public void GetCommission(object sender, EventArgs eventArgs);
        public void ChargeInterest(object sender, EventArgs eventArgs);
    }
}