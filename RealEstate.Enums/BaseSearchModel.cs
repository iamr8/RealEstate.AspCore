using Microsoft.AspNetCore.Mvc;
using RealEstate.Base.Attributes;

namespace RealEstate.Base
{
    public abstract class BaseSearchModel
    {
        [SearchParameter("pageNo")]
        [HiddenInput]
        public int PageNo { get; set; }
    }
}