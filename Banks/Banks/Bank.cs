using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Clients;
using Banks.Tools;

namespace Banks.Banks
{
    public class Bank : IBank
    {
        private readonly List<Client> _clients;

        public Bank(string name, Dictionary<AccountType, object> interestedRates, Dictionary<AccountType, double> commissions)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Bank name mustn't be null or empty");
            Name = name;
            InterestedRates = interestedRates ?? throw new ArgumentNullException(nameof(interestedRates));
            Commissions = commissions ?? throw new ArgumentNullException(nameof(commissions));
            _clients = new List<Client>();
        }

        public string Name { get; }
        public Dictionary<AccountType, object> InterestedRates { get; }
        public Dictionary<AccountType, double> Commissions { get; }

        public Client AddClient(ClientInfo clientInfo)
        {
            var client = new Client(clientInfo);
            _clients.Add(client);
            return client;
        }

        public void AddAccount(Client client, Account account)
        {
            account.IsDoubtful = client.Info.PassportNumber == null || client.Info.Address == null;
            WaybackMachine.Instance.NewDay += account.OnNewDay;
            client.Accounts.Add(account);
        }

        public Client FindClientByPhoneNumber(string num) => _clients.Find(client => client.Info.PhoneNumber == num);

        public void GetCommission(object sender, EventArgs eventArgs) => _clients.ForEach(client => client.Accounts.ForEach(account => account.PayCommission()));

        public void ChargeInterest(object sender, EventArgs eventArgs) => _clients.ForEach(client => client.Accounts.ForEach(account => account.ChargeInterest()));
    }
}