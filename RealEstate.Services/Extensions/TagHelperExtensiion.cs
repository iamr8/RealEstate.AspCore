using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions
{
    public static class TagHelperExtensiion
    {
        public static ModelExpression GetPropertyModelExpression(this ModelExpression model, string propertyName)
        {
            return new ModelExpression($"{model.Name}.{propertyName}", model.ModelExplorer.GetExplorerForProperty(propertyName));
        }

        public static async Task<TagBuilder> RenderTagHelperAsync<THelper>(this THelper tagHelper) where THelper : TagHelper
        {
            var tagName = tagHelper.GetType().GetCustomAttribute<HtmlTargetElementAttribute>().Tag;

            var attributes = tagHelper.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new TagHelperAttribute(x.Name, x.GetValue(tagHelper)))
                .ToList();

            var context = new TagHelperContext(new TagHelperAttributeList(attributes), new Dictionary<object, object>(), Guid.NewGuid().ToString());
            var output = new TagHelperOutput(
                tagName,
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                    Task.Factory.StartNew<TagHelperContent>(
                        () => new DefaultTagHelperContent()
                    ));

            await tagHelper.ProcessAsync(context, output);

            var createdTag = new TagBuilder(output.TagName);
            createdTag.MergeAttributes(output.Attributes.ToDictionary(c => c.Name, c => c.Value));
            createdTag.InnerHtml.SetContent(output.Content.GetContent());

            return createdTag;
        }
    }
}