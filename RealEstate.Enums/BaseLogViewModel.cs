using Newtonsoft.Json;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public class BaseLogViewModel : BaseViewModel
    {
        [JsonIgnore]
        public List<LogViewModel> Logs { get; set; }

        [JsonIgnore]
        public LogViewModel LastLog => Logs?.OrderByDescending(x => x.DateTime).FirstOrDefault();
    }

    public static class BaseTrackExtension
    {
        public static LogViewModel Last(this List<LogViewModel> logs)
        {
            return logs?.OrderByDescending(x => x.DateTime).FirstOrDefault();
        }
    }

    public class LogViewModel
    {
        public LogTypeEnum Type { get; set; }
        public DateTime DateTime { get; set; }
        public string Id { get; set; }
        public LogUserViewModel User { get; set; }
    }

    public class LogUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Role Role { get; set; }
    }
}