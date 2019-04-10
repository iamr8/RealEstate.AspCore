using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class Permission : BaseEntity
    {
        public PermissionTypeEnum Type { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}