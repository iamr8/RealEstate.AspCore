namespace RealEstate.Base
{
    public class BaseInputViewModel : BaseViewModel
    {
        public bool IsNew => string.IsNullOrEmpty(Id);
    }
}