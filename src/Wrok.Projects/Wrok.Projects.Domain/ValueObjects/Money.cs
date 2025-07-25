using Wrok.Projects.Domain.Enums;

namespace Wrok.Projects.Domain.ValueObjects;
public record struct Money(decimal Amount, Currency Currency = Currency.USD)
{
    public static Money Zero => new(0);

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException("Cannot add money with different currencies");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }
};
