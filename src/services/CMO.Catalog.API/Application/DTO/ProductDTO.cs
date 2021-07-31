using CMO.Core.Validators;
using FluentValidation;
using System;

namespace CMO.Product.API.Application.DTO
{
    public class ProductDTO : ValidateDTO
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public override bool Validate()
        {
            ValidationResult = new ProductDTOValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class ProductDTOValidation : AbstractValidator<ProductDTO>
        {
            public ProductDTOValidation()
            {
                RuleFor(c => c.Title)
                    .NotEmpty()
                    .WithMessage("O t�tulo do produto n�o foi informado");

                RuleFor(c => c.Description)
                    .NotEmpty()
                    .WithMessage("A descri��o do produto n�o foi informada");

                RuleFor(c => c.Price)
                    .NotEmpty()
                    .WithMessage("O pre�o do produto n�o foi informado");

                RuleFor(c => c.Quantity)
                    .NotEmpty()
                    .WithMessage("A quantidade do produto n�o foi informada");
            }
        }

    }
}