using System;
using Banks.Accounts;

namespace Banks.InterestedRates
{
    public class DebitInterestedRate : IInterestedRate
    {
        private readonly double _interestedRate;

        public DebitInterestedRate(double interestedRate)
        {
            if (interestedRate <= 0)
                throw new ArgumentException($"{nameof(interestedRate)} must be bigger than 0");
            _interestedRate = interestedRate;
        }

        public double GetInterestedRateFor(double sum) => _interestedRate;
    }
}