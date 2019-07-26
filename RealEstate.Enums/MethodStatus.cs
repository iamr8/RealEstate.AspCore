using RealEstate.Base.Enums;

namespace RealEstate.Base
{
    public class MethodStatus<TEntity>
    {
        public void Deconstruct(out StatusEnum status, out TEntity entity, out bool isSuccess)
        {
            status = Status;
            entity = Entity;
            isSuccess = (Status == StatusEnum.Success || Status == StatusEnum.NoNeedToSave) && Entity != null;
        }

        public void Deconstruct(out StatusEnum status, out TEntity entity)
        {
            status = Status;
            entity = Entity;
        }

        public MethodStatus(StatusEnum status, TEntity entity = default, string tag = null)
        {
            Tag = tag;
            Status = status;
            Entity = entity;
        }

        public MethodStatus()
        {
        }

        public StatusEnum Status { get; set; }
        public TEntity Entity { get; set; }
        public string Tag { get; set; }
    }
}