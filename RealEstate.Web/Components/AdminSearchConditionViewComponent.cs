using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RealEstate.Base;

namespace RealEstate.Web.Components
{
    public class AdminSearchConditionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ModelExpression model)
        {
            var test = new AdminSearchConditionViewModel();

            var searchModel = model.Model;
            if (searchModel == null)
                return View(null);

            var searchModelType = searchModel.GetType();
            var properties = searchModelType.GetPublicProperties();

            var creatorIdProp = properties.Find(x => x.Name == nameof(test.CreatorId));
            var creationDateFromProp = properties.Find(x => x.Name == nameof(test.CreationDateFrom));
            var creationDateToProp = properties.Find(x => x.Name == nameof(test.CreationDateTo));
            var includeDeletedItemsProp = properties.Find(x => x.Name == nameof(test.IncludeDeletedItems));

            var creatorId = creatorIdProp.GetValue(searchModel) as string;
            var dateFrom = creationDateFromProp.GetValue(searchModel) as string;
            var dateTo = creationDateToProp.GetValue(searchModel) as string;
            var includeDeleted = includeDeletedItemsProp.GetValue(searchModel) is bool && (bool)includeDeletedItemsProp.GetValue(searchModel);

            var result = new AdminSearchConditionWrapperViewModel
            {
                SearchInput = new AdminSearchConditionViewModel
                {
                    CreatorId = creatorId,
                    CreationDateFrom = dateFrom,
                    CreationDateTo = dateTo,
                    IncludeDeletedItems = includeDeleted
                }
            };
            return View(result);
        }
    }
}