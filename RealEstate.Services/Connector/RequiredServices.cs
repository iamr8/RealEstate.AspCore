using Microsoft.Extensions.DependencyInjection;

namespace RealEstate.Services.Connector
{
    public static class RequiredServices
    {
        public static void AddRequiredServices(this IServiceCollection service)
        {
            service.AddScoped<IBaseService, BaseService>();
            service.AddScoped<IUserService, UserService>();
            //            service.AddScoped<ILocationService, LocationService>();
            //            service.AddScoped<IItemService, ItemService>();
            service.AddScoped<IContactService, ContactService>();
            //            service.AddScoped<IPaymentService, PaymentService>();
            //            service.AddScoped<IDealService, DealService>();
            //            service.AddScoped<IFeatureService, FeatureService>();
            //            service.AddScoped<ISmsService, SmsService>();
            //            service.AddScoped<IPictureService, PictureService>();
            service.AddScoped<IPropertyService, PropertyService>();
            service.AddScoped<IPictureService, PictureService>();
            service.AddScoped<IFileHandler, FileHandler>();
        }
    }
}