using Fluid;
using Fluid.Ast;
using Fluid.Tags;
using System.Text.Encodings.Web;

namespace Deals.Business.FluentLiquidTemplate.Tags
{
    internal sealed class RegisterSectionBlock : IdentifierBlock
    {
        private static readonly ValueTask<Completion> Normal = new ValueTask<Completion>(Completion.Normal);

        public override ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, string sectionName, List<Statement> statements)
        {
            if (context.AmbientValues.TryGetValue("Sections", out var sections))
            {
                var dictionary = (Dictionary<string, List<Statement>>) sections;
                dictionary[sectionName] = statements;
            }

            return Normal;
        }
    }
}
