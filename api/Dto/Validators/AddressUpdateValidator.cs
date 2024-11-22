using api.Dto;
using FluentValidation;

namespace api.Dto.Validators
{
    public class AddressUpdateValidator : AbstractValidator<AddressUpdate>
    {
        public AddressUpdateValidator()
        {
            RuleFor(address => address.Street).NotEmpty();
            RuleFor(address => address.PostalCode).NotEmpty();
            RuleFor(address => address.City).NotEmpty();
            RuleFor(address => address.Country).NotEmpty();
        }
    }
}
