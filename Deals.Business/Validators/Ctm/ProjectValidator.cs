using DTO.Ctm;
using FluentValidation;

namespace Deals.Business.Validators.Ctm
{
    public class AddProjectValidator : AbstractValidator<AddProjectDTO>
    {
        public AddProjectValidator()
        {
            RuleFor(x => x.ClientName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ProjectCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.SbuId)
              .NotNull().WithMessage("SbuId can't be empty.")
              .GreaterThan(0).WithMessage("SbuId must be greater than 0.");
            RuleFor(x => x.LegalEntityid)
              .NotNull().WithMessage("LegalEntityid can't be empty.")
              .GreaterThan(0).WithMessage("LegalEntityid must be greater than 0.");


        }
    }

    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDTO>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.ClientName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ProjectCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.SbuId).NotNull().WithMessage("SbuId can't be empty.")
            .GreaterThan(0).WithMessage("SbuId must be greater than 0.");
            RuleFor(x => x.LegalEntityid).NotNull().WithMessage("LegalEntityid can't be empty.")
            .GreaterThan(0).WithMessage("LegalEntityid must be greater than 0.");

        }
    }
}
