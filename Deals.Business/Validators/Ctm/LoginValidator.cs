using DTO.Ctm;
using FluentValidation;

namespace Deals.Business.Validators.Ctm
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.DomainName).NotEmpty().MaximumLength(20);
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Password).NotEmpty().MaximumLength(20);
        }
    }
}
