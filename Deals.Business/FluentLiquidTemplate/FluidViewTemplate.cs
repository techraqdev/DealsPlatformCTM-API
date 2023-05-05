using Deals.Business.FluentLiquidTemplate.Tags;

using Fluid;

namespace Deals.Business.FluentLiquidTemplate
{
    public class FluidViewTemplate : BaseFluidTemplate<FluidViewTemplate>
    {
        static FluidViewTemplate()
        {
            Factory.RegisterTag<LayoutTag>("layout");
            Factory.RegisterTag<RenderBodyTag>("renderbody");
            Factory.RegisterBlock<RegisterSectionBlock>("section");
            Factory.RegisterTag<RenderSectionTag>("rendersection");
        }
    }
}
