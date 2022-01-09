using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.Accounts.Debit;
using Banks.Accounts.Deposit;
using Banks.Banks;
using Banks.Clients;
using Banks.Service;
using Banks.Transactions;
using static Banks.UI.Ui;

namespace Banks.UI
{
    public class ClientMainLoop : IMainLoop
    {
        private readonly IBanksService _service;
        private IBank _bank;
        private Client _client;

        public ClientMainLoop(IBanksService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Run()
        {
            if (!ClientEnter(out _bank, out _client))
                return;

            bool needContinue = true;
            var menu = new Menu("Choose action", new List<Menu.Item>
            {
                new Menu.Item("Add account", AddAccount),
                new Menu.Item("Display history", DisplayHistory),
                new Menu.Item("Make transaction", MakeTransaction),
                new Menu.Item("Get balance", GetBalance),
                new Menu.Item("Exit", () => needContinue = false),
            });

            while (needContinue)
                menu.Render();
        }

        private static Client SignIn(IBank bank)
        {
            string ans;
            do
            {
                Console.Write("Enter your phone: ");
                string phone = Console.ReadLine();
                Client client = bank.FindClientByPhoneNumber(phone);
                if (client != null)
                    return client;
                Console.Error.WriteLine("Can't find client with such phone number.");
                Console.Write("Do you want to continue? [Y/n] ");
                ans = Console.ReadLine();
            }
            while (ans != "n");

            return null;
        }

        private static Client SignUp(IBank bank)
        {
            while (true)
            {
                try
                {
                    var clientInfo = default(ClientInfo);
                    Console.Write("Enter your phone: ");
                    clientInfo.PhoneNumber = Console.ReadLine();
                    Console.Write("Enter your first name: ");
                    clientInfo.FirstName = Console.ReadLine();
                    Console.Write("Enter your last name: ");
                    clientInfo.LastName = Console.ReadLine();
                    Console.Write("Enter your passport number (optional): ");
                    clientInfo.PassportNumber = Console.ReadLine();
                    Console.Write("Enter your address (optional): ");
                    clientInfo.Address = Console.ReadLine();
                    return bank.AddClient(clientInfo);
                }
                catch (ArgumentException e)
                {
                    Console.Error.WriteLine($"{e.Message}.\n");
                }
            }
        }

        private static Account GetAccount(Client client)
        {
            Account account = null;
            var menu = new Menu("Choose account type", new List<Menu.Item>
            {
                new Menu.Item("Debit", () => account = client.Accounts.Find(acc => acc.Type == AccountType.Debit)),
                new Menu.Item("Credit", () => account = client.Accounts.Find(acc => acc.Type == AccountType.Credit)),
                new Menu.Item("Deposit", () => account = client.Accounts.Find(acc => acc.Type == AccountType.Deposit)),
            });
            menu.Render();
            return account;
        }

        private void AddAccount()
        {
            Account account = null;
            var menu = new Menu("Choose account type", new List<Menu.Item>
            {
                new Menu.Item("Debit", () =>
                {
                    if (_client.Accounts.Find(acc => acc.Type == AccountType.Debit) != null)
                        Console.Error.WriteLine("Debit account already exists\n");
                    else
                        account = new Debit(_client, _bank);
                }),
                new Menu.Item("Credit", () =>
                {
                    if (_client.Accounts.Find(acc => acc.Type == AccountType.Credit) != null)
                    {
                        Console.Error.WriteLine("Credit account already exists\n");
                        return;
                    }

                    Console.Write("Enter a credit limit: ");
                    double limit;
                    try
                    {
                        limit = Convert.ToDouble(Console.ReadLine());
                        if (limit < 0)
                        {
                            Console.WriteLine("Credit limit can't be negative\n");
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid input\n");
                        return;
                    }

                    account = new Credit(_client, _bank, limit);
                }),
                new Menu.Item("Deposit", () =>
                {
                    if (_client.Accounts.Find(acc => acc.Type == AccountType.Deposit) != null)
                    {
                        Console.Error.WriteLine("Deposit account already exists\n");
                        return;
                    }

                    Console.Write("Enter deposit validity: ");
                    try
                    {
                        account = new Deposit(_client, _bank, Convert.ToUInt64(Console.ReadLine()));
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("Invalid input\n");
                    }
                }),
            });
            menu.Render();
            if (account == null)
                return;
            _bank.AddAccount(_client, account);
            Console.WriteLine("Account has been successfully added.\n");
        }

        private bool ClientEnter(out IBank bank, out Client client)
        {
            client = null;

            bank = GetClientBank();
            if (bank == null)
                return false;

            // can't use out parameter in lambda
            IBank bankCopy = bank;
            Client clientCopy = client;

            var menu = new Menu("Choose action", new List<Menu.Item>
                {
                    new Menu.Item("Login", () => clientCopy = SignIn(bankCopy)),
                    new Menu.Item("Signup", () => clientCopy = SignUp(bankCopy)),
                });
            menu.Render(true);

            client = clientCopy;
            return client != null;
        }

        private IBank GetClientBank()
        {
            while (true)
            {
                Console.Write("Enter your bank name: ");
                IBank bank = _service.FindBank(Console.ReadLine());
                if (bank != null)
                    return bank;
                Console.Write("Can't find the bank. Do you want to continue? [Y/n] ");
                string ans = Console.ReadLine();
                Console.WriteLine();
                if (ans == "n")
                    return null;
            }
        }

        private void MakeTransaction()
        {
            Account account = GetAccount(_client);
            if (account == null)
            {
                Console.Error.WriteLine("You don't have such account\n");
                return;
            }

            Console.Write("Enter the sum: ");
            int sum;
            try
            {
                sum = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Invalid input.\n");
                return;
            }

            Transaction transaction = null;
            var menu = new Menu("Choose transaction type", new List<Menu.Item>
            {
                new Menu.Item("Withdraft", () => transaction = new Withdraft(account, sum)),
                new Menu.Item("Top up", () => transaction = new TopUp(account, sum)),
                new Menu.Item("Transfer", () => transaction = Transfer(account, sum)),
            });
            menu.Render();
            if (transaction != null && !transaction.Commit(out string err))
                Console.Error.WriteLine($"{err}\n");
            else
                Console.WriteLine("Successful transaction\n");
        }

        private Transaction Transfer(Account account, int sum)
        {
            Client receiver;
            do
            {
                Console.Write("Enter receiver number: ");
                string number = Console.ReadLine();
                receiver = _bank.FindClientByPhoneNumber(number);
                if (receiver != null)
                    break;
                Console.Write("Can't find such client. Do you want to continue? [Y/n] ");
                if (Console.ReadLine() == "n")
                    return null;
            }
            while (true);

            Account receiverAccount = GetAccount(receiver);
            if (receiverAccount == null)
            {
                Console.Error.WriteLine("Receiver doesn't have such account.\n");
                return null;
            }

            return new Transfer(account, receiverAccount, sum);
        }

        private void GetBalance()
        {
            Account account = GetAccount(_client);
            if (account == null)
                Console.Error.WriteLine("You don't have such account\n");
            else
                Console.WriteLine($"Balance: {account.Balance}\n");
        }

        private void DisplayHistory()
        {
            if (_client.History.Count == 0)
            {
                Console.WriteLine("No transactions\n");
                return;
            }

            const int step = 15;
            int nHistoryItems = _client.History.Count;
            int index = 0;
            while (nHistoryItems > 0)
            {
                bool itIsLastPage = nHistoryItems - step < 0;
                int currStep = nHistoryItems < step ? nHistoryItems : step;
                uint offset = (uint)index;
                List<Transaction> transactions = _client.History.GetRange(index, step);
                Console.Clear();
                Console.WriteLine("Choose transaction to rollback: ");
                transactions.ForEach(transaction => Console.WriteLine($"{index++}. {transaction.ToString()}"));

                if (itIsLastPage)
                {
                    Console.WriteLine("It's last page.");
                }
                else
                {
                    Console.Write("Next page? [Y/n] ");
                    if (Console.ReadLine() == "n")
                    {
                        nHistoryItems -= currStep;
                        continue;
                    }
                }

                uint transactionIndex = Convert.ToUInt32(Console.ReadLine()) - offset;

                if (transactions.Count < transactionIndex)
                {
                    Console.Error.WriteLine(UnknownOptionMsg);
                    return;
                }

                if (!transactions[(int)transactionIndex].Rollback(out string err))
                {
                    Console.WriteLine($"Can't rollback transaction: {err}\n");
                    return;
                }

                Console.WriteLine("Transaction successful rollback\n");
                return;
            }
        }
    }
}