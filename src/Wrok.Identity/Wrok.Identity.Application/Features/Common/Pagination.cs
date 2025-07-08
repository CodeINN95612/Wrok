using FluentValidation;

namespace Wrok.Identity.Application.Features.Common;
public abstract class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public abstract class PaginationResponse<T> where T : class
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int? Next => 
        TotalCount > (PageNumber * PageSize) ? PageNumber + 1 : null;
    public int? Previous =>
        PageNumber > 1 ? PageNumber - 1 : null;
    public List<T> Data { get; set; } = [];
}

public sealed class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}