using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;

namespace Banks.InterestedRates
{
    public class DepositInterestedRate : IInterestedRate
    {
        private readonly List<(double, double)> _interestedRatesBySum;
        private readonly double _interestRateForMinSum;

        public DepositInterestedRate(double interestRateForMinSum)
        {
            _interestRateForMinSum = interestRateForMinSum;
            _interestedRatesBySum = new List<(double, double)>();
        }

        public bool Add(double sum, double interestedRate, out string errDesc)
        {
            errDesc = null;

            if (AlreadyAdded(interestedRate))
            {
                errDesc = $"Interested rate {interestedRate} already added";
                return false;
            }

            _interestedRatesBySum.Add((interestedRate, sum));
            Sort();

            return true;
        }

        public double GetInterestedRateFor(double sum)
        {
            foreach ((double interestedRate, double currSum) in _interestedRatesBySum.ToArray().Reverse())
            {
                if (sum > currSum)
                    return interestedRate;
            }

            return _interestRateForMinSum;
        }

        private bool AlreadyAdded(double interestedRate)
        {
            foreach ((double currInterestedRate, double _) in _interestedRatesBySum)
            {
                if (currInterestedRate == interestedRate)
                    return true;
            }

            return false;
        }

        private void Sort()
        {
            _interestedRatesBySum.Sort((interestedRateBySum1, interestedRateBySum2) =>
            {
                if (interestedRateBySum1.Item1 > interestedRateBySum2.Item1)
                    return 1;
                else if (interestedRateBySum1.Item1 == interestedRateBySum2.Item1)
                    return 0;
                else
                    return -1;
            });
        }
    }
}