using RealEstate.Base.Enums;

namespace RealEstate.Base
{
    public class MethodStatus<TEntity>
    {
        public void Deconstruct(out StatusEnum status, out TEntity entity)
        {
            status = Status;
            entity = Entity;
        }

        public MethodStatus(StatusEnum status, TEntity entity)
        {
            Status = status;
            Entity = entity;
        }

        public MethodStatus()
        {
        }

        public StatusEnum Status { get; set; }
        public TEntity Entity { get; set; }
    }
}