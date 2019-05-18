using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.BaseLog
{
    public class LogDetailViewModel
    {
        public LogTypeEnum Type { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserMobile { get; set; }
        public Dictionary<string, string> Modifies { get; set; }
    }
}