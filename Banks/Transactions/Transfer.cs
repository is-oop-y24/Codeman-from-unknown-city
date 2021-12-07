using System;
using Banks.Accounts;

namespace Banks.Transactions
{
    public class Transfer : Transaction
    {
        private readonly Account _sender;
        private readonly Account _receiver;

        public Transfer(Account sender, Account receiver, double sum)
            : base(sum, "Transfer")
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        public override string ToString() =>
            $"Transfer from {_sender.Owner.Info.PhoneNumber} to {_receiver.Owner.Info.PhoneNumber}: {Sum}";

        protected override bool Commit(double sum, out string errDesc)
        {
            bool err = TransferMoney(_sender, _receiver, sum, out errDesc);
            if (!err)
                return false;
            _sender.Owner.History.Add(this);
            _receiver.Owner.History.Add(this);
            return true;
        }

        protected override bool Rollback(double sum, out string errDesc)
        {
            bool err = TransferMoney(_receiver, _sender, sum, out errDesc);
            if (err)
                return false;
            _sender.Owner.History.Remove(this);
            _receiver.Owner.History.Remove(this);
            return true;
        }

        private static bool TransferMoney(Account from, Account to, double sum, out string errDesc)
        {
            if (!from.Send(sum, out errDesc))
                return false;

            if (!to.Receive(sum, out errDesc))
            {
                from.Receive(sum, out errDesc);
                return false;
            }

            return true;
        }
    }
}