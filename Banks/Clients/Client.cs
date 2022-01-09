using System.Collections.Generic;
using Banks.Accounts;
using Banks.Transactions;

namespace Banks.Clients
{
    public class Client
    {
        public Client(ClientInfo info)
        {
            Info = info;
            Accounts = new List<Account>();
            History = new List<Transaction>();
        }

        public ClientInfo Info { get; set; }
        public List<Account> Accounts { get; set; }

        public List<Transaction> History { get; set; }
    }
}