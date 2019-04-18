using Microsoft.Extensions.DependencyInjection;
using RealEstate.Services.Extensions.KavenNegarProvider;

namespace RealEstate.Services.Base
{
    public static class RequiredServices
    {
        public static void AddRequiredServices(this IServiceCollection service)
        {
            // Base
            service.AddScoped<IBaseService, BaseService>();
            service.AddScoped<IFileHandler, FileHandler>();
            //            service.AddScoped<IMapService, MapService>();

            // Sms Provider
            service.AddScoped<IKavehNegarProvider, KavehNegarProvider>();

            // Services Implementation
            service.AddScoped<ICustomerService, CustomerService>();
            service.AddScoped<IDealService, DealService>();
            service.AddScoped<IFeatureService, FeatureService>();
            service.AddScoped<IItemService, ItemService>();
            service.AddScoped<ILocationService, LocationService>();
            service.AddScoped<IPaymentService, PaymentService>();
            service.AddScoped<IPictureService, PictureService>();
            service.AddScoped<IPropertyService, PropertyService>();
            service.AddScoped<ISmsService, SmsService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IEmployeeService, EmployeeService>();
        }
    }
}