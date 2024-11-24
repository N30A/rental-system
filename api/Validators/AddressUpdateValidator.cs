using api.Models.Dtos;
using FluentValidation;

namespace api.Validators
{
    public class AddressUpdateValidator : AbstractValidator<UpdateAddressRequest>
    {
        public AddressUpdateValidator()
        {
            RuleFor(address => address.Street)
                .NotEmpty()
                .WithMessage("Street is required.");

            RuleFor(address => address.PostalCode)
                .NotEmpty()
                .WithMessage("PostalCode is required.");

            RuleFor(address => address.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(address => address.Country)
                .NotEmpty()
                .WithMessage("Country is required.");
        }
    }
}
