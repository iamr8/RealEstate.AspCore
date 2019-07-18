using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class R8InputTagHelper : TagHelper
    {
        public R8InputTagHelper(IHtmlGenerator generator)
        {
            this.Generator = generator;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        protected IHtmlGenerator Generator { get; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var validation = For
                .Metadata
                .ValidatorMetadata
                .Where(x => x.GetType() == typeof(ValueValidationAttribute))
                .Select(x => x as ValueValidationAttribute)
                .FirstOrDefault();
            if (validation == null)
                return;
            if (!output.IsContentModified)
            {
                output.Attributes.Add("data-toggle", "tooltip");
                output.Attributes.Add("data-placement", "bottom");
                output.Attributes.Add("title", validation.ErrorMessage);
                output.Attributes.Add("placeholder", validation.Pattern.GetDescription());
            }
        }
    }
}