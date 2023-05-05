using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using FluentValidation;

namespace Deals.Business.Validators
{
    public class AddProjectCredValidator : AbstractValidator<AddProjectCredDTO>
    {
        public AddProjectCredValidator()
        {
            RuleFor(x => x.BusinessDescription).MaximumLength(500);
            RuleFor(x => x.TargetEntityName).MaximumLength(50);
        }
    }

    public class UpdateProjectCredValidator : AbstractValidator<UpdateProjectCredDTO>
    {

        public UpdateProjectCredValidator()
        {
            RuleFor(x => x.BusinessDescription).MaximumLength(500);
            RuleFor(x => x.TargetEntityName).MaximumLength(50);

        }
    }
}
