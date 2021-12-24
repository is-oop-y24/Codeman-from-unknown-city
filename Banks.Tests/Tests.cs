using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Accounts.Debit;
using Banks.Accounts.Deposit;
using Banks.Clients;
using Banks.Service;
using Banks.Tools;
using Banks.Transactions;
using NUnit.Framework;

namespace Banks.Tests
{
    public class Tests
    {
        private const ulong OnWhichDayNotifyAboutCommission = 30;
        private const string ClientPhoneNumber = "+7 (914) 234-2312";
        private IBanksService _banksService;
        private IBank _bank;

        private static Dictionary<AccountType, object> InstallInterestedRates()
        {
            var depositInterestedRate = new InterestedRate(3);
            depositInterestedRate.Add(50000, 3.5, out string _);
            depositInterestedRate.Add(100000, 4, out string _);
            return new Dictionary<AccountType, object>
            {
                [AccountType.Debit] = 4,
                [AccountType.Deposit] = depositInterestedRate
            };
        }

        private static Dictionary<AccountType, double> InstallCommissions() => new Dictionary<AccountType, double>
            {
                [AccountType.Credit] = 1000
            };
        
        [SetUp]
        public void Setup()
        {
            _banksService = new BanksService(OnWhichDayNotifyAboutCommission);
            _bank = _banksService.AddBank("Tinkoff", InstallInterestedRates(), InstallCommissions(), out string _);
        }

        [Test]
        public void GiveWrongInfo_ThrowException()
        {
            var clientInfo = new ClientInfo();
            Assert.Catch<ArgumentException>(() => clientInfo.FirstName = "");
            Assert.Catch<ArgumentException>(() => clientInfo.LastName = "");
            Assert.Catch<ArgumentException>(() => clientInfo.PhoneNumber = "23412334");
            Assert.Catch<ArgumentException>(() => clientInfo.PassportNumber = "2431223412334");
        }

        private Client AddClient()
        {
            var info = new ClientInfo
            {
                PhoneNumber = ClientPhoneNumber,
                FirstName = "Alex",
                LastName = "Black",
                PassportNumber = "002321",
                Address = "Street, 10"
            };
            return _bank.AddClient(info);
        }
        
        [Test]
        public void AddClient_ClientAdded()
        {
            AddClient();
            Assert.True(_bank.FindClientByPhoneNumber(ClientPhoneNumber) != null);
        }

        private Account AddAccount(Client client)
        {
            Account account = new Debit(client, _bank);
            _bank.AddAccount(client, account);
            return account;
        }

        [Test]
        public void AddAccount_AccountAdded()
        {
            Client client = AddClient();
            AddAccount(client);
            Assert.True(client.Accounts.Find(account => account.Type == AccountType.Debit) != null);
        }

        [Test]
        public void MakeTransaction_TransactionSuccess()
        {
            Client client = AddClient();
            Account account = AddAccount(client);
            const double sum = 5000;
            Transaction topUp = new TopUp(account, sum);
            Assert.True(topUp.Commit(out string _) && client.History.Contains(topUp) && account.Balance == sum);
        }

        [Test]
        public void RollbackTransaction_TransactionRollbacked()
        {
            Client client = AddClient();
            Account account = AddAccount(client);
            const double sum = 5000;
            Transaction topUp = new TopUp(account, sum);
            Assert.True(topUp.Commit(out string _) && 
                        topUp.Rollback(out string _) &&
                        !client.History.Contains(topUp) &&
                        account.Balance == 0);
        }

        [Test]
        public void RewindTime_BalanceNotChanged()
        {
            Client client = AddClient();
            var account = new Deposit(client, _bank, 100);
            const double sum = 500;
            new TopUp(account, sum).Commit(out string _);
            var waybackMachine = WaybackMachine.Instance;
            waybackMachine.RewindTimeForwardOnDay();
            Assert.True(account.Balance == sum);
        }

        [Test]
        public void RewindTime_InterestedAccrued()
        {
            
            Client client = AddClient();
            var account = new Deposit(client, _bank, 100);
            _bank.AddAccount(client, account);
            const double sum = 500;
            new TopUp(account, sum).Commit(out string _);
            var waybackMachine = WaybackMachine.Instance;
            waybackMachine.RewindTimeForwardOnMount();
            double expectedBalance = sum + sum / 100 * (3d / 365d) * 29;
            Assert.True(account.Balance == expectedBalance);
        }
    }
}