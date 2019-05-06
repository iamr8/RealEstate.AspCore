using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class StatisticsViewModel
    {
        public List<StatisticsDetailViewModel> Today { get; set; }
        public List<StatisticsDetailViewModel> ThisWeek { get; set; }
        public List<StatisticsDetailViewModel> ThisMonth { get; set; }
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