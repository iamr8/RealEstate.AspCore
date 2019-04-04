using RealEstate.Base.Enums;
using RealEstate.Domain.Base;

namespace RealEstate.Domain.Tables
{
    public class Permission : BaseEntity
    {
        public PermissionTypeEnum Type { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}