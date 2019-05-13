using RealEstate.Base.Enums;

namespace RealEstate.Base
{
    public class ModelStateValidation
    {
        public ModelStateValidation(StatusEnum status, string message)
        {
            Status = status;
            Message = message;
        }

        public ModelStateValidation()
        {
        }

        public void Deconstruct(out StatusEnum status, out string message)
        {
            status = Status;
            message = Message;
        }

        public StatusEnum Status { get; set; }
        public string Message { get; set; }
    }
}