using Wrok.Projects.Domain.Exceptions;

namespace Wrok.Projects.Domain.ValueObjects;

public sealed class HourlyRateRange
{
    public Money Min { get; private set; }
    public Money Max { get; private set; }

    private HourlyRateRange(Money min, Money max)
    {
        Min = min;
        Max = max;
    }

    public static HourlyRateRange Create(Money min, Money max)
    {
        if (min.Amount <= 0 || max.Amount <= 0)
        {
            DomainException.ThrowInvalid(nameof(HourlyRateRange), nameof(Min), "Min and Max must be greater than zero.");
        }
        if (min.Amount >= max.Amount)
        {
            DomainException.ThrowInvalid(nameof(HourlyRateRange), nameof(Min), "Min must be less than Max.");
        }
        if(min.Currency != max.Currency)
        {
            DomainException.ThrowInvalid(nameof(HourlyRateRange), nameof(Min), "Min and Max must have the same currency.");
        }
        return new(min, max);
    }
}
