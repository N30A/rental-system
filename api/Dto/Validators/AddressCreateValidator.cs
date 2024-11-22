using api.Dto;
using FluentValidation;

namespace api.Dto.Validators
{
    public class AddressCreateValidator : AbstractValidator<AddressCreate>
    {
        public AddressCreateValidator()
        {
            RuleFor(address => address.Street).NotEmpty();
            RuleFor(address => address.PostalCode).NotEmpty();
            RuleFor(address => address.City).NotEmpty();
            RuleFor(address => address.Country).NotEmpty();
        }
    }
}
