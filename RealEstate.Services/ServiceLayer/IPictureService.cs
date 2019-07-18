using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.ModelBind;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IPictureService
    {
        Task<StatusEnum> PictureAddAsync(IFormFile[] pictures, string text, string propertyId, string dealId, string employeeId, string paymentId,
            string reminderId, bool save);

        Task<StatusEnum> PictureRemoveAsync(string pictureId);

        Task<List<PictureViewModel>> PropertyPicturesAsync(string id);
    }

    public class PictureService : IPictureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IFileHandler _fileHandler;
        private readonly DbSet<Picture> _pictures;

        public PictureService(
            IUnitOfWork unitOfWork,
            IFileHandler fileHandler,
            IBaseService baseService
            )
        {
            _unitOfWork = unitOfWork;
            _fileHandler = fileHandler;
            _baseService = baseService;
            _pictures = _unitOfWork.Set<Picture>();
        }

        public async Task<List<PictureViewModel>> PropertyPicturesAsync(string id)
        {
            var query = await _pictures.Where(x => x.PropertyId == id).ToListAsync();
            if (query?.Any() != true)
                return default;

            var result = query.Select(x => x.Map<PictureViewModel>()).ToList();
            return result;
        }

        public async Task<StatusEnum> PictureRemoveAsync(string pictureId)
        {
            if (string.IsNullOrEmpty(pictureId))
                return StatusEnum.ParamIsNull;

            var entity = await _pictures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == pictureId);
            var result = await _baseService.RemoveAsync(entity,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> PictureAddAsync(IFormFile[] pictures, string text, string propertyId, string dealId, string employeeId, string paymentId, string reminderId, bool save)
        {
            if (pictures?.Any() != true)
                return StatusEnum.Success;

            var files = await _fileHandler.SaveAsync(pictures);
            if (files?.Any() != true)
                return StatusEnum.FileIsNull;

            var results = new List<StatusEnum>();
            foreach (var picture in files)
            {
                var (newStatus, newPicture) = await _baseService.AddAsync(new Picture
                {
                    PropertyId = !string.IsNullOrEmpty(propertyId) ? propertyId : null,
                    DealId = !string.IsNullOrEmpty(dealId) ? dealId : null,
                    EmployeeId = !string.IsNullOrEmpty(employeeId) ? employeeId : null,
                    PaymentId = !string.IsNullOrEmpty(paymentId) ? paymentId : null,
                    ReminderId = !string.IsNullOrEmpty(reminderId) ? reminderId : null,
                    Text = text,
                    File = picture,
                }, null, save);
                results.Add(newStatus);
            }

            return results.Populate();
        }
    }
}