using FluentValidation.Results;
using System;

namespace CMO.Core.Validators
{

    public abstract class ValidateDTO
    {
        public ValidationResult ValidationResult { get; set; }

        protected ValidateDTO()
        {
            ValidationResult = new ValidationResult();
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }
    }

}
