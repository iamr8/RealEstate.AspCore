using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base
{
    public class AdminSearchConditionViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Creator")]
        [SearchParameter("creatorId")]
        public string CreatorId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FromDate")]
        [SearchParameter("dateFrom")]
        public string CreationDateFrom { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ToDate")]
        [SearchParameter("dateTo")]
        public string CreationDateTo { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ShowDeletedItems")]
        [SearchParameter("deleted")]
        public bool IncludeDeletedItems { get; set; }
    }
}