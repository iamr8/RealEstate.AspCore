using Microsoft.AspNetCore.Mvc;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base
{
    public abstract class BaseSearchModel
    {
        [SearchParameter("pageNo")]
        [HiddenInput]
        public int PageNo { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "ShowDeletedItems")]
        [SearchParameter("deleted")]
        public bool IncludeDeletedItems { get; set; }
    }
}