using Android.App;
using Android.Content;

namespace RealEstate.Droid.Utils
{
    public class App : Application
    {
        private static Context _mContext;

        public override void OnCreate()
        {
            _mContext = Context;
        }

        public static Context GetAppContext()
        {
            return _mContext;
        }
    }
}