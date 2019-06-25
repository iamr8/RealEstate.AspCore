using Android.App;
using Android.OS;

namespace RealEstate.Droid
{
    [Activity(Label = "@string/signin", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class Initialize : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.initialize);
            // Create your application here
        }
    }
}