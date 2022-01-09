using System;
using System.Collections.Generic;
using Banks.Accounts;
using Banks.InterestedRates;
using Banks.Service;

namespace Banks.UI
{
    public class BankerMainLoop : IMainLoop
    {
        private readonly IBanksService _service;

        public BankerMainLoop(IBanksService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public void Run()
        {
            bool needContinue = true;
            var menu = new Menu("Choose action", new List<Menu.Item>
            {
                new Menu.Item("Add bank", AddBank),
                new Menu.Item("Exit", () => needContinue = false),
            });
            while (needContinue)
                menu.Render();
        }

        private static Dictionary<AccountType, IInterestedRate> GetInterestedRates()
        {
            var interestedRates = new Dictionary<AccountType, IInterestedRate>();
            Console.Write("Enter interested rate for debit account: ");
            interestedRates.Add(AccountType.Debit, new DebitInterestedRate(Convert.ToDouble(Console.ReadLine())));
            var depositInterestedRates = new DepositInterestedRate(0);
            Console.WriteLine("Enter interested rate for deposit account:");
            do
            {
                Console.Write("If sum bigger than ");
                double sum = Convert.ToDouble(Console.ReadLine());
                Console.Write("Then interested rate will be = ");
                double interestedRate = Convert.ToDouble(Console.ReadLine());
                if (!depositInterestedRates.Add(sum, interestedRate, out string errDesc))
                    Console.Error.WriteLine(errDesc);
                Console.Write("Do you want to continue? [Y/n] ");
            }
            while (Console.ReadLine() != "n");
            interestedRates.Add(AccountType.Deposit, depositInterestedRates);
            return interestedRates;
        }

        private static Dictionary<AccountType, double> GetCommissions()
        {
            var commissions = new Dictionary<AccountType, double>();
            Console.Write("Enter commission for debit account: ");
            commissions.Add(AccountType.Debit, Convert.ToDouble(Console.ReadLine()));
            Console.Write("Enter commission for credit account: ");
            commissions.Add(AccountType.Credit, Convert.ToDouble(Console.ReadLine()));
            return commissions;
        }

        private void AddBank()
        {
                string err = null;
                do
                {
                    if (err != null)
                        Console.Error.WriteLine($"{err}\n");

                    Console.Write("Enter a bank name: ");
                    string bankName = Console.ReadLine();

                    if (_service.FindBank(bankName) != null)
                    {
                        err = "The bank is already exists";
                        continue;
                    }

                    _service.AddBank(bankName, GetInterestedRates(), GetCommissions(), out err);
                }
                while (err != null);
        }
    }
}