using RealEstate.Services.ViewModels.Api.Response;

namespace RealEstate.Services.ViewModels.Api
{
    public class TokenCheck : ResponseWrapper
    {
        public UserResponse User { get; set; }
    }
}