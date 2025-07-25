using Wrok.Projects.Domain.Enums;
using Wrok.Projects.Domain.Exceptions;
using Wrok.Projects.Domain.ValueObjects;

namespace Wrok.Projects.Domain.Entities;

public abstract class PricingModel
{
    public PricingModelId Id { get; private set; }
    public PricingModelType PricingModelType { get; private set; }

    public PricingModel(PricingModelType type)
    {
        Id = new PricingModelId(Guid.CreateVersion7());
        PricingModelType = type;
    }

#nullable disable
    private PricingModel() { } // For EF
#nullable enable
}

public sealed class FixedBudgetPricingModel : PricingModel
{
    public Money Budget { get; private set; }
    public FixedBudgetPricingModel(Money budget) : base(PricingModelType.FixedBudget)
    {
        if (budget.Amount <= 0)
        {
            DomainException.ThrowInvalid(nameof(FixedBudgetPricingModel), nameof(Budget), "Budget must be greater than zero.");
        }
        Budget = budget;
    }

#nullable disable
    private FixedBudgetPricingModel() : base(PricingModelType.FixedBudget) { } // For EF
#nullable enable
}

public sealed class HourlyRatePricingModel : PricingModel
{
    public HourlyRateRange? Range { get; private set; }
    public bool IsDefined => Range is not null;
    
    private HourlyRatePricingModel(HourlyRateRange? range) : base(PricingModelType.HourlyRate) 
    {
        Range = range;
    }

    public static HourlyRatePricingModel CreateUnbounded()
    {
        return new(null);
    }

    public static HourlyRatePricingModel CreateWithRange(Money min, Money max)
    {
        return new(HourlyRateRange.Create(min, max));
    }

#nullable disable
    private HourlyRatePricingModel() : base(PricingModelType.HourlyRate) { } // For EF
#nullable enable
}
