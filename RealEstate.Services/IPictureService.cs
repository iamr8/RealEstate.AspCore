using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IPictureService
    {
        Task<(StatusEnum, Picture)> PropertyPictureAddAsync(PropertyPhotoInputViewModel model);
    }

    public class PictureService : IPictureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandler _fileHandler;
        private readonly DbSet<Picture> _pictures;

        public PictureService(
            IUnitOfWork unitOfWork,
            IFileHandler fileHandler
            )
        {
            _unitOfWork = unitOfWork;
            _fileHandler = fileHandler;
            _pictures = _unitOfWork.PlugIn<Picture>();
        }

        public async Task<(StatusEnum, Picture)> PropertyPictureAddAsync(PropertyPhotoInputViewModel model)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.ModelIsNull, null);

            // Upload Picture
            var file = _fileHandler.Upload(model.File);
            if (file == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.FileIsNull, null);

            var newPicture = new Picture
            {
                PropertyId = model.PropertyId,
                Text = model.Text,
                File = file.File,
            };

            var save = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false) > 0;
            return save
                ? new ValueTuple<StatusEnum, Picture>(StatusEnum.Success, newPicture)
                : new ValueTuple<StatusEnum, Picture>(StatusEnum.UnableToSave, newPicture);
        }
    }
}