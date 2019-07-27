using System.Collections.Generic;

namespace RealEstate.Base
{
    public class PaginationViewModel<TModel> : PaginationViewModel where TModel : class
    {
        public PaginationViewModel()
        {
            Items = default;
            CurrentPage = 1;
            Pages = 1;
        }

        public List<TModel> Items { get; set; }
    }

    public abstract class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public int Rows { get; set; }
    }
}