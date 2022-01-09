using System;
using System.Collections.Generic;
using Banks.Banks;
using Banks.Clients;
using Banks.InterestedRates;

namespace Banks.Accounts.Deposit
{
    public class Deposit : Account
    {
        private readonly ulong _validity;
        private double _interest;
        private ulong _daysCounter;

        public Deposit(Client owner, IBank bank, ulong validity, bool isDoubtful = true)
            : base(owner, bank)
        {
            _interest = 0;
            _daysCounter = 0;
            _validity = validity;
            Balance = 0;
            Type = AccountType.Deposit;
        }

        public override void PayCommission()
        { }

        public override void ChargeInterest()
        {
            Balance += _interest;
            _interest = 0;
        }

        public override void OnNewDay(object sender, EventArgs eventArgs)
        {
            _interest += Balance / 100 * (Bank.InterestedRates[Type].GetInterestedRateFor(Balance) / 365);
            _daysCounter++;
        }

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
            if (_daysCounter < _validity)
            {
                errDesc = $"You will be able to withdraw money only after {_validity - _daysCounter} days";
                return false;
            }

            return ChangeBalance(-sum, out errDesc);
        }

        protected override bool SendMoney(double sum, out string errDesc)
        {
            if (_daysCounter < _validity)
            {
                errDesc = $"You will be able to withdraw money only after {_validity - _daysCounter} days";
                return false;
            }

            return ChangeBalance(-sum, out errDesc);
        }

        private bool ChangeBalance(double sum, out string errDesc)
        {
            errDesc = null;

            if (Balance + sum < 0)
            {
                errDesc = "Deposit account can't be negative";
                return false;
            }

            Balance += sum;
            return true;
        }
    }
}