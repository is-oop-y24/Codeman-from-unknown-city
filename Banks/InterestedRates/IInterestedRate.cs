using Banks.Accounts;

namespace Banks.InterestedRates
{
    public interface IInterestedRate
    {
        double GetInterestedRateFor(double sum);
    }
}