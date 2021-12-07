using System;

namespace Banks.Transactions
{
    public abstract class Transaction
    {
        private readonly string _name;
        private bool _isCommitted;
        private bool _isRollbacked;

        protected Transaction(double sum, string name)
        {
            Sum = sum;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Transaction name mustn't be null or empty");
            _name = name;
        }

        public double Sum { get; }
        public Guid Id { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transaction)obj);
        }

        public bool Equals(Transaction other) => Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();

        public bool Commit(out string errDesc)
        {
            if (_isCommitted)
            {
                errDesc = $"{_name} has already been completed";
                return false;
            }

            _isCommitted = Commit(Sum, out errDesc);
            return _isCommitted;
        }

        public bool Rollback(out string errDesc)
        {
            if (!_isCommitted)
            {
                errDesc = $"{_name} hasn't been completed";
                return false;
            }

            if (_isRollbacked)
            {
                errDesc = $"{_name} has already been rollbacked";
                return false;
            }

            _isRollbacked = Rollback(Sum, out errDesc);
            return _isRollbacked;
        }

        public new abstract string ToString();

        protected abstract bool Commit(double sum, out string errDesc);
        protected abstract bool Rollback(double sum, out string errDesc);
    }
}