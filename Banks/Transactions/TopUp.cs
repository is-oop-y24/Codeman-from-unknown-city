using System;
using Banks.Accounts;

namespace Banks.Transactions
{
    public class TopUp : Transaction
    {
        private readonly Account _account;

        public TopUp(Account account, double sum)
            : base(sum, "Top up")
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public override string ToString() => $"Top up: {Sum}";

        protected override bool Commit(double sum, out string errDesc)
        {
            if (!_account.TopUp(sum, out errDesc))
                return false;

            _account.Owner.History.Add(this);
            return true;
        }

        protected override bool Rollback(double sum, out string errDesc)
        {
            if (!_account.Withdraft(sum, out errDesc))
                return false;

            _account.Owner.History.Remove(this);
            return true;
        }
    }
}