using FluentValidation;

namespace UniversityMS.Application.Features.Inventory.Commands;

public class CreateStockItemCommandValidator : AbstractValidator<CreateStockItemCommand>
{
    public CreateStockItemCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Depo ID boş olamaz");

        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Malzeme kodu boş olamaz")
            .Length(2, 50).WithMessage("Malzeme kodu 2-50 karakter arasında olmalı");

        RuleFor(x => x.ItemName)
            .NotEmpty().WithMessage("Malzeme adı boş olamaz")
            .MaximumLength(200).WithMessage("Malzeme adı 200 karakteri geçemez");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kategori boş olamaz");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Miktar negatif olamaz");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Birim fiyatı 0'dan büyük olmalı");
    }
}