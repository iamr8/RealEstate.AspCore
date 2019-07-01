using Microsoft.Extensions.Localization;
using Moq;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Services.ViewModels;
using Xunit;

namespace RealEstate.Services.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task ItemListSearchTest()
        {
            var itemMoq = new Mock<IItemService>();
            var baseMoq = new Mock<IBaseService>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var propertyMoq = new Mock<IPropertyService>();
            var userMoq = new Mock<IUserService>();
            var customerMoq = new Mock<ICustomerService>();
            var localizerMoq = new Mock<IStringLocalizer<SharedResource>>();

            var searchModel = new ItemSearchViewModel();
            baseMoq
                .Setup(service => service.IsAllowed(It.IsAny<Role>()))
                .Returns(true);
            //baseMoq
            //    .Setup(service => service.CheckDeletedItemsPrevillege(It.IsAny<DbSet<Item>>(), searchModel, out CurrentUserViewModel null))
            //    .Returns((IQueryable<Item> query) => query);

            itemMoq
                .Setup(service => service.ItemListAsync(It.IsAny<ItemSearchViewModel>(), false))
                .Returns(Task.FromResult(FakeItemsList()));

            var itemService = new ItemService(baseMoq.Object, unitOfWork.Object, propertyMoq.Object, userMoq.Object, customerMoq.Object, localizerMoq.Object);

            var obj = itemMoq.Object;
            var items = await obj.ItemListAsync(new ItemSearchViewModel());
            Assert.Null(items);
        }

        private PaginationViewModel<ItemViewModel> FakeItemsList()
        {
            var pagination = new PaginationViewModel<ItemViewModel>
            {
                CurrentPage = 1,
                Pages = 1,
                Rows = 10,
            };

            var items = new List<ItemViewModel>();

            var itemEntity = new Item
            {
                Id = Guid.NewGuid().ToString(),
                PropertyId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
            };
            var item = new ItemViewModel(itemEntity);
            items.Add(item);

            pagination.Items = items;
            return pagination;
        }
    }
}