using System.Collections.Generic;

namespace RealEstate.Base
{
    public class PaginationViewModel<TModel> where TModel : class
    {
        public PaginationViewModel()
        {
            Items = default;
            CurrentPage = 1;
            Pages = 1;
        }

        public List<TModel> Items { get; set; }
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public bool HasDuplicates { get; set; }
    }
}