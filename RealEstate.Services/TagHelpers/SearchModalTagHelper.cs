using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewComponents;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("search-modal", Attributes = "searchModel", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SearchModalTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IViewComponentHelper _viewComponentHelper;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("searchModel")]
        public ModelExpression SearchModel { get; set; }

        public IHtmlGenerator Generator { get; }

        public SearchModalTagHelper(
            IStringLocalizer<SharedResource> localizer,
            IViewComponentHelper viewComponentHelper,
            IHtmlGenerator generator
            )
        {
            _localizer = localizer;
            _viewComponentHelper = viewComponentHelper;
            Generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync()).GetContent();

            var modalTitle = new TagBuilder("h5");
            modalTitle.AddCssClass("modal-title");
            modalTitle.InnerHtml.AppendHtml(_localizer[SharedResource.SearchFilters]);

            var closeText = new TagBuilder("span");
            closeText.Attributes.Add("aria-hidden", "true");
            closeText.InnerHtml.AppendHtml("&times;");

            var closeButton = new TagBuilder("button");
            closeButton.Attributes.Add("type", "button");
            closeButton.Attributes.Add("data-dismiss", "modal");
            closeButton.Attributes.Add("aria-label", "Close");
            closeButton.AddCssClass("close");
            closeButton.InnerHtml.AppendHtml(closeText);

            var modalHeader = new TagBuilder("div");
            modalHeader.AddCssClass("modal-header");
            modalHeader.InnerHtml.AppendHtml(modalTitle).AppendHtml(closeButton);

            var modalContent = new TagBuilder("div");
            modalContent.AddCssClass("modal-content");
            modalContent.InnerHtml.AppendHtml(modalHeader);

            var modalDialog = new TagBuilder("div");
            modalDialog.AddCssClass("modal-dialog");
            modalDialog.AddCssClass("modal-dialog-centered");
            modalDialog.Attributes.Add("role", "document");

            var form = new TagBuilder("form");
            form.Attributes.Add("method", "post");

            var antiforgery = Generator.GenerateAntiforgery(ViewContext);
            form.InnerHtml.AppendHtml(antiforgery);

            var modalFooter = new TagBuilder("div");
            modalFooter.AddCssClass("modal-footer");

            var hasPrevillege = ViewContext.HttpContext.User.IsInRole(nameof(Role.Admin)) || ViewContext.HttpContext.User.IsInRole(nameof(Role.SuperAdmin));
            if (hasPrevillege)
            {
                BaseSearchModel search;
                var includeDeletedItemsProperty = SearchModel.Model.GetType().GetPublicProperties()
                    .FirstOrDefault(x => x.Name.Equals(nameof(search.IncludeDeletedItems), StringComparison.CurrentCulture));

                var deletedCheck = await new InputTagHelper(Generator)
                {
                    ViewContext = ViewContext,
                    For = SearchModel.GetPropertyModelExpression(nameof(search.IncludeDeletedItems))
                }.RenderTagHelperAsync();
                deletedCheck.AddCssClass("custom-control-input");

                var deletedCheckLabel = await new LabelTagHelper(Generator)
                {
                    ViewContext = ViewContext,
                    For = SearchModel.GetPropertyModelExpression(nameof(search.IncludeDeletedItems))
                }.RenderTagHelperAsync();

                deletedCheckLabel.AddCssClass("custom-control-label");
                deletedCheckLabel.InnerHtml.Clear().AppendHtml(includeDeletedItemsProperty.GetDisplayName());

                var deletedCheckWrapper = new TagBuilder("div");
                deletedCheckWrapper.AddCssClass("custom-control");
                deletedCheckWrapper.AddCssClass("custom-switch");
                deletedCheckWrapper.Attributes.Add("dir", "rtl");
                deletedCheckWrapper.InnerHtml.AppendHtml(deletedCheck).AppendHtml(deletedCheckLabel);

                var customControlWrapper = new TagBuilder("div");
                customControlWrapper.AddCssClass("ml-auto");
                customControlWrapper.InnerHtml.AppendHtml(deletedCheckWrapper);
                modalFooter.InnerHtml.AppendHtml(customControlWrapper);
            }

            var cancelButton = new TagBuilder("button");
            cancelButton.Attributes.Add("type", "button");
            cancelButton.Attributes.Add("data-dismiss", "modal");
            cancelButton.AddCssClass("btn");
            cancelButton.AddCssClass("btn-secondary");
            cancelButton.AddCssClass("btn-sm");
            cancelButton.InnerHtml.AppendHtml(_localizer[SharedResource.Cancel]);

            var submitButton = new TagBuilder("button");
            submitButton.Attributes.Add("type", "submit");
            submitButton.AddCssClass("btn");
            submitButton.AddCssClass("btn-primary");
            submitButton.AddCssClass("btn-sm");
            submitButton.InnerHtml.AppendHtml(_localizer[SharedResource.Search]);

            var modalBody = new TagBuilder("div");
            modalBody.AddCssClass("modal-body");

            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            var adminSearch = await _viewComponentHelper.InvokeAsync(typeof(AdminSearchConditionViewComponent), new { model = SearchModel });

            var modalBodyContainer = new TagBuilder("div");
            modalBodyContainer.AddCssClass("container-fluid");
            modalBodyContainer.InnerHtml.AppendHtml(content).AppendHtml(adminSearch);

            modalBody.InnerHtml.AppendHtml(modalBodyContainer);

            modalFooter.InnerHtml.AppendHtml(cancelButton).AppendHtml(submitButton);
            form.InnerHtml.AppendHtml(modalBody).AppendHtml(modalFooter);
            modalContent.InnerHtml.AppendHtml(form);

            modalDialog.InnerHtml.AppendHtml(modalContent);
            output.Content.AppendHtml(modalDialog);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.AddClass("modal", HtmlEncoder.Default);
            output.AddClass("fade", HtmlEncoder.Default);
            output.AddClass("search", HtmlEncoder.Default);
            output.Attributes.Add("id", "searchModal");
            output.Attributes.Add("tabindex", "-1");
            output.Attributes.Add("role", "dialog");
            output.Attributes.Add("aria-labelledby", "searchModalLabel");
            output.Attributes.Add("aria-hidden", "true");
        }
    }
}