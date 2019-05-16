using RealEstate.Base.Enums;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public static class PopulateStatusesExtensions
    {
        public static StatusEnum Populate(this List<StatusEnum> statuses)
        {
            StatusEnum result;
            if (statuses.All(x => x == StatusEnum.Success))
                result = StatusEnum.Success;
            else if (statuses.Any(x => x == StatusEnum.Success))
                result = StatusEnum.PartialSuccess;
            else
                result = StatusEnum.Failed;
            return result;
        }
    }
}