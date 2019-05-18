using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RealEstate.Services.TagHelpers
{
    public class TagHelperCutPaste
    {
        public const string ItemsStorageKey = "a2b459c4-3c62-4a90-977a-5999eb5978c5";

        // CutPasteKey identifies the appartenence of the cuted part to the paste section
        public string CutPasteKey { get; set; }

        // Cut script TagContent
        public TagHelperContent TagHelperContent { get; set; }

        // Attributes belonging to the cut script
        public List<TagHelperAttribute> Attributes { get; set; }
    }

    [HtmlTargetElement("script", Attributes = "asp-cut-key")]
    public class TagHelperScriptCut : TagHelper
    {
        [HtmlAttributeName("asp-cut-key")]
        public string CutKey { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var deferedScripts = new List<TagHelperCutPaste>();

            if (ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPaste.ItemsStorageKey))
            {
                deferedScripts = ViewContext.HttpContext.Items[TagHelperCutPaste.ItemsStorageKey] as List<TagHelperCutPaste>;
                if (deferedScripts == null)
                    throw new ApplicationException("Duplicate Items key : " + TagHelperCutPaste.ItemsStorageKey);
            }
            else
            {
                ViewContext.HttpContext.Items.Add(TagHelperCutPaste.ItemsStorageKey, deferedScripts);
            }

            var result = await output.GetChildContentAsync().ConfigureAwait(false);
            deferedScripts.Add(new TagHelperCutPaste
            {
                CutPasteKey = this.CutKey,
                TagHelperContent = result,
                Attributes = context.AllAttributes.Where(x => x.Name != "asp-cut-key").ToList()
            });

            output.Content.Clear();
            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("script", Attributes = "asp-paste-key")]
    public class TagHelperScriptPaste : TagHelper
    {
        [HtmlAttributeName("asp-paste-key")]
        public string DeferDestinationId { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private IHtmlGenerator Generator { get; set; }

        public TagHelperScriptPaste(IHtmlGenerator generator)
        {
            this.Generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output).ConfigureAwait(false);

            if (!this.ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPaste.ItemsStorageKey))
                return;

            var storage = ViewContext.HttpContext.Items[TagHelperCutPaste.ItemsStorageKey];
            if (!(storage is List<TagHelperCutPaste> cutKeys))
                throw new ApplicationException($"Conversion failed for item type {storage.GetType()} to type" + typeof(Dictionary<string, TagHelperCutPaste>));

            if (cutKeys.Count == 0)
                return;

            var componentsWithStorageKey = cutKeys.Where(x => x.CutPasteKey == DeferDestinationId).ToList();
            if (componentsWithStorageKey.Count == 0)
                return;

            var firstScript = componentsWithStorageKey.First();
            output.Content.SetHtmlContent(firstScript.TagHelperContent.GetContent());

            foreach (var attr in firstScript.Attributes)
                output.Attributes.Add(attr);

            if (componentsWithStorageKey.Count <= 0)
                return;

            for (var i = 1; i < componentsWithStorageKey.Count; i++)
            {
                var script = componentsWithStorageKey[i];
                var builder = new TagBuilder("script");

                builder.MergeAttributes(script.Attributes.ToDictionary(x => x.Name, x => x.Value));
                builder.InnerHtml.AppendHtml(script.TagHelperContent.GetContent());
                output.PostElement.AppendHtml(builder);
            }
        }
    }
}