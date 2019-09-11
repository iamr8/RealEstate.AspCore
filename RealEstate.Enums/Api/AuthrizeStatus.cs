using System;
using Newtonsoft.Json;

namespace RealEstate.Base.Api
{
    public class AuthorizeStatus : ResponseStatus
    {
        [JsonIgnore]
        public UserResponse User { get; set; }

        [JsonIgnore]
        public Version Version { get; set; }

        [JsonIgnore]
        public OS UserOs { get; set; }

        [JsonIgnore]
        public Device Device { get; set; }
    }
}