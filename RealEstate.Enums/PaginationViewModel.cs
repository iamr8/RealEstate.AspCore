using System.Collections.Generic;

namespace RealEstate.Base
{
    public class PaginationViewModel<TModel> where TModel : class
    {
        public List<TModel> Items { get; set; }
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
    }
}