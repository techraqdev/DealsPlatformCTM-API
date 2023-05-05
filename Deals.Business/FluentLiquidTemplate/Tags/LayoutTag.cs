using Fluid;
using Fluid.Ast;
using Fluid.Tags;
using System.Text.Encodings.Web;

namespace Deals.Business.FluentLiquidTemplate.Tags
{
    internal sealed class LayoutTag : ExpressionTag
    {
        private const string ViewExtension = ".liquid";

        public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, Expression expression)
        {
            var relativeLayoutPath = (await expression.EvaluateAsync(context)).ToStringValue();
            if (!relativeLayoutPath.EndsWith(ViewExtension, StringComparison.OrdinalIgnoreCase))
            {
                relativeLayoutPath += ViewExtension;
            }

            context.AmbientValues["Layout"] = relativeLayoutPath;
            return Completion.Normal;
        }
    }
}
