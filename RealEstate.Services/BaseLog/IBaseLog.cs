using Newtonsoft.Json;

namespace RealEstate.Services.BaseLog
{
    public interface IBaseLog<T>
    {
        [JsonIgnore]
        T Entity { get; set; }
    }
}