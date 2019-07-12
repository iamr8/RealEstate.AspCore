using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class R8LabelTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var required = For.Metadata.IsRequired;
            if (required)
                output.Attributes.Add("required", "required");
        }
    }
}