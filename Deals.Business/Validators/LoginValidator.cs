using DTO;
using FluentValidation;

namespace Deals.Business.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            //RuleFor(x => x.DomainName).NotEmpty().MaximumLength(20);
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MaximumLength(20);
        }
    }
}
