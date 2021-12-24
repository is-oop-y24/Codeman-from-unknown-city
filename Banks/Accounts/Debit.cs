using System;
using Banks.Clients;

namespace Banks.Accounts.Debit
{
    public class Debit : Account
    {
        private double _interest;

        public Debit(Client owner, IBank bank)
            : base(owner, bank)
        {
            Type = AccountType.Debit;
            Balance = 0;
            _interest = 0;
        }

        public override void PayCommission()
        { }

        public override void ChargeInterest()
        {
            Balance += _interest;
        }

        public override void OnNewDay(object sender, EventArgs eventArgs)
        {
            int interestRate = (int)Bank.InterestedRates[Type];
            _interest += Balance / 100 * interestRate;
        }

        protected override bool ReceiveMoney(double sum, out string errDesc) => ChangeBalance(sum, out errDesc);

        protected override bool TopUpMoney(double sum, out string errDesc) => ChangeBalance(sum, out errDesc);
        protected override bool SendMoney(double sum, out string errDesc) => ChangeBalance(-sum, out errDesc);
        protected override bool WithdraftMoney(double sum, out string errDesc) => ChangeBalance(-sum, out errDesc);

        private bool ChangeBalance(double sum, out string errDesc)
        {
            errDesc = null;

            if (Balance + sum < 0)
            {
                errDesc = "Debit account can't be negative";
                return false;
            }

            Balance += sum;
            return true;
        }
    }
}