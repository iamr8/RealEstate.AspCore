using RealEstate.Services.ViewModels.Api.Response;

namespace RealEstate.Services.ViewModels.Api
{
    public class TokenCheck : ResponseStatus
    {
        public UserResponse User { get; set; }
    }
}