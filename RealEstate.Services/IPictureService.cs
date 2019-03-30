using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;
using RealEstate.Services.Base;

namespace RealEstate.Services
{
    public interface IPictureService
    {
        Task<(StatusEnum, Picture)> PropertyPictureAddAsync(PropertyPhotoInputViewModel model, bool save, UserViewModel currentUser);
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
            _pictures = _unitOfWork.PlugIn<Picture>();
        }

        public async Task<(StatusEnum, Picture)> PropertyPictureAddAsync(PropertyPhotoInputViewModel model, bool save, UserViewModel currentUser)
        {
            currentUser = _baseService.CurrentUser(currentUser);
            if (currentUser == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.UserIsNull, null);

            if (model == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.ModelIsNull, null);

            // Upload Picture
            var file = _fileHandler.Upload(model.File);
            if (file == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.FileIsNull, null);

            var newPicture = _unitOfWork.Add(new Picture
            {
                PropertyId = model.PropertyId,
                Text = model.Text,
                File = file.File,
            });

            return await _baseService.SaveChangesAsync(newPicture, save).ConfigureAwait(false);
        }
    }
}