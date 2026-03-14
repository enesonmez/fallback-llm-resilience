using FallbackLLMResilience.API.DTOs;
using FluentValidation;

namespace FallbackLLMResilience.API.Validators;

public class GenerateContentRequestValidator : AbstractValidator<GenerateContentRequest>
{
    public GenerateContentRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Ürün adı 100 karakteri geçemez.");

        RuleFor(x => x.Features)
            .NotEmpty().WithMessage("En az bir ürün özelliği belirtilmelidir.");
        
        RuleForEach(x => x.Features).SetValidator(new ProductFeatureValidator());
    }
}

public class ProductFeatureValidator : AbstractValidator<ProductFeature>
{
    public ProductFeatureValidator()
    {
        RuleFor(x => x.Name) // Özellik adı (örn: Materyal)
            .NotEmpty().WithMessage("Özellik anahtarı boş olamaz.");

        RuleFor(x => x.Value) // Özellik değeri (örn: %100 Pamuk)
            .NotEmpty().WithMessage("Özellik değeri boş olamaz.");
    }
}