using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.BaseLog
{
    public class LogViewModel
    {
        public LogDetailViewModel Create { get; set; }
        public List<LogDetailViewModel> Modifies { get; set; }
        public List<LogDetailViewModel> Deletes { get; set; }

        public LogDetailViewModel Last
        {
            get
            {
                var populate = new List<LogDetailViewModel>();

                if (Create != null)
                    populate.Add(Create);

                if (Modifies?.Any() == true)
                    populate.AddRange(Modifies);

                if (Deletes?.Any() == true)
                    populate.AddRange(Deletes);

                populate = populate.OrderByDescending(x => x.DateTime).ToList();
                return populate.FirstOrDefault();
            }
        }
    }
}