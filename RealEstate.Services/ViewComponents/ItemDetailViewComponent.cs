using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Json;
using System.Threading.Tasks;

namespace RealEstate.Services.ViewComponents
{
    public class ItemDetailViewComponent : ViewComponent
    {
        private readonly IItemService _itemService;

        public ItemDetailViewComponent(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View("~/Pages/Components/ItemDetail/Default.cshtml", new ItemOutJsonViewModel());

            var model = await _itemService.ItemJsonAsync(id);
            return View("~/Pages/Components/ItemDetail/Default.cshtml", model);
        }
    }
}