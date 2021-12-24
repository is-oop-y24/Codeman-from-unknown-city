using System;
using Banks.Banks;
using Banks.Clients;

namespace Banks.Accounts
{
    public abstract class Account
    {
        protected Account(Clients.Client owner, IBank bank)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Bank = bank ?? throw new ArgumentNullException(nameof(bank));
        }

        public AccountType Type { get; protected set; }
        public bool IsDoubtful { get => Owner.Info.IsDoubtful; }
        public double AllowedSendSum { get; set; }
        public Client Owner { get; }
        public double Balance { get; protected set; }
        protected IBank Bank { get; }

        public abstract void PayCommission();
        public abstract void ChargeInterest();

        public bool Withdraft(double sum, out string errDesc)
        {
            if (!IsItValidSum("Withdraft", sum, out errDesc))
                return false;

            if (IsDoubtful)
            {
                errDesc = "You can't withdraft money from doubtful account";
                return false;
            }

            return WithdraftMoney(sum, out errDesc);
        }

        public bool Send(double sum, out string errDesc)
        {
            if (!IsItValidSum("Send", sum, out errDesc))
                return false;

            if (IsDoubtful && sum > AllowedSendSum)
            {
                errDesc = $"You can't send sum than bigger than {AllowedSendSum} on doubtful account";
                return false;
            }

            return SendMoney(sum, out errDesc);
        }

        public bool Receive(double sum, out string errDesc)
        {
            if (!IsItValidSum("Receive", sum, out errDesc))
                return false;

            return ReceiveMoney(sum, out errDesc);
        }

        public bool TopUp(double sum, out string errDesc)
        {
            if (!IsItValidSum("Top up", sum, out errDesc))
                return false;

            return TopUpMoney(sum, out errDesc);
        }

        public abstract void OnNewDay(object sender, EventArgs eventArgs);
        protected abstract bool ReceiveMoney(double sum, out string errDesc);
        protected abstract bool TopUpMoney(double sum, out string errDesc);
        protected abstract bool WithdraftMoney(double sum, out string errDesc);
        protected abstract bool SendMoney(double sum, out string errDesc);

        private static bool IsItValidSum(string methodName, double sum, out string errDesc)
        {
            bool itIsBiggerThanNull = sum > 0;
            errDesc = itIsBiggerThanNull ? null : $"{methodName} sum must be bigger than 0";
            return itIsBiggerThanNull;
        }
    }
}