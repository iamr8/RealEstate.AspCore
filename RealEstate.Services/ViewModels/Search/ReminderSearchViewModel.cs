using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class ReminderSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("subj")]
        public string Subject { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CheckBank")]
        [SearchParameter("chkb")]
        public string CheckBank { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "CheckNumber")]
        [SearchParameter("chkn")]
        public string CheckNumber { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Price")]
        [SearchParameter("prc")]
        public decimal? Price { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "From")]
        [SearchParameter("from")]
        public string FromDate { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "To")]
        [SearchParameter("to")]
        public string ToDate { get; set; }
    }
}