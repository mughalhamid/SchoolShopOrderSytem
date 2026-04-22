using FluentValidation;
using SchoolShopOrderSystem.Application.DTO;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be greater than 0");

        RuleFor(x => x.MenuItems)
            .NotEmpty()
            .WithMessage("At least one menu item is required");
    }
}
