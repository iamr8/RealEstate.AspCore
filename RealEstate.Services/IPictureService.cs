using Microsoft.EntityFrameworkCore;
using RealEstate.Base.Enums;
using RealEstate.Domain;
using RealEstate.Domain.Tables;
using RealEstate.Services.Base;
using System;
using System.Threading.Tasks;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services
{
    public interface IPictureService
    {
        Task<(StatusEnum, Picture)> PictureAddAsync(PictureInputViewModel model, PictureTypeEnum type, string targetId, bool save);
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

        public async Task<(StatusEnum, Picture)> PictureAddAsync(PictureInputViewModel model, PictureTypeEnum type, string targetId, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.ModelIsNull, null);

            // Upload Picture
            var file = _fileHandler.Upload(model.File);
            if (file == null)
                return new ValueTuple<StatusEnum, Picture>(StatusEnum.FileIsNull, null);

            var newPicture = await _baseService.AddAsync(new Picture
            {
                PropertyId = type == PictureTypeEnum.Property ? targetId : null,
                DealPaymentId = type == PictureTypeEnum.DealPayment ? targetId : null,
                PaymentId = type == PictureTypeEnum.PaymentId ? targetId : null,
                DealId = type == PictureTypeEnum.Deal ? targetId : null,
                Text = model.Text,
                File = file.File,
            }, null, save).ConfigureAwait(false);
            return newPicture;
        }
    }
}