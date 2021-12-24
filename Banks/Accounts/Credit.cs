using System;
using Banks.Clients;

namespace Banks.Accounts
{
    public class Credit : Account
    {
        public Credit(Client owner, IBank bank, double limit = 0)
            : base(owner, bank)
        {
            Type = AccountType.Credit;
            Balance = limit;
        }

        public override void PayCommission()
        {
            Balance -= Bank.Commissions[Type];
        }

        public override void ChargeInterest()
        { }

        public override void OnNewDay(object sender, EventArgs eventArgs)
        { }

        protected override bool ReceiveMoney(double sum, out string errDesc)
        {
            errDesc = null;
            Balance += sum;
            return true;
        }

        protected override bool TopUpMoney(double sum, out string errDesc)
        {
            errDesc = null;
            Balance += sum;
            return true;
        }

        protected override bool WithdraftMoney(double sum, out string errDesc)
        {
            errDesc = null;
            Balance -= sum;
            return true;
        }

        protected override bool SendMoney(double sum, out string errDesc)
        {
            errDesc = null;
            Balance -= sum;
            return true;
        }
    }
}