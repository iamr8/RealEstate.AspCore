using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class StatisticsViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<StatisticsDetailViewModel> Details { get; set; }
        public StatisticsRangeEnum Range { get; set; }
    }

    public class StatisticsDetailViewModel
    {
        public string ItemId { get; set; }
        public string ItemCategory { get; set; }
        public string PropertyCategory { get; set; }
        public string UserFullName { get; set; }
        public string UserId { get; set; }
    }
}