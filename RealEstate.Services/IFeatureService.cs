using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using RealEstate.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IFeatureService
    {
        Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category);
    }

    public class FeatureService : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IMapService _mapService;
        private readonly DbSet<UserPropertyCategory> _userPropertyCategories;
        private readonly DbSet<UserItemCategory> _userItemCategories;
        private readonly DbSet<Category> _categories;

        public FeatureService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IMapService mapService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _mapService = mapService;
            _userPropertyCategories = _unitOfWork.Set<UserPropertyCategory>();
            _userItemCategories = _unitOfWork.Set<UserItemCategory>();
            _categories = _unitOfWork.Set<Category>();
        }

        public async Task<List<CategoryViewModel>> CategoryListAsync(CategoryTypeEnum? category)
        {
            var query = _categories as IQueryable<Category>;

            if (category != null)
                query = query.Where(x => x.Type == category);

            var categories = await query.ToListAsync().ConfigureAwait(false);
            return _mapService.Map(categories);
        }
    }
}