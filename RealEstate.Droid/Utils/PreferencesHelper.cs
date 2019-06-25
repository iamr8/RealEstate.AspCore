using Android.Content;
using Newtonsoft.Json;
using RealEstate.Droid.Models;

namespace RealEstate.Droid.Utils
{
    public class PreferencesHelper
    {
        private static PreferencesHelper _instance;
        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _editor;
        private const string Settings = "settings";
        private const string MainConfig = "main_config";
        private const string UserToken = "user_token";
        private const string BaseUrl = "base_url";

        public static PreferencesHelper GetInstance()
        {
            if (_instance == null)
                _instance = new PreferencesHelper();

            var preferenceHelper = _instance;

            return preferenceHelper;
        }

        private void Init()
        {
            if (_preferences != null)
                return;

            _preferences = App.GetAppContext().GetSharedPreferences(Settings, 0);
            _editor = _preferences.Edit();
        }

        public void SetMainConfig(MainViewModel model)
        {
            if (model == null)
                return;

            var serialize = JsonConvert.SerializeObject(model);
            _editor.PutString(MainConfig, serialize);
            _editor.Commit();
        }

        public MainViewModel GetMainConfig()
        {
            var mainConfigJson = _preferences.GetString(MainConfig, "");
            if (string.IsNullOrEmpty(mainConfigJson))
                return default;

            var model = JsonConvert.DeserializeObject<MainViewModel>(mainConfigJson);
            return model;
        }

        public void SetBaseUrl(string baseUrl)
        {
            _editor.PutString(BaseUrl, baseUrl);
            _editor.Commit();
        }

        public string GetBaseUrl()
        {
            Init();
            return _preferences.GetString(BaseUrl, "");
        }

        public void SetToken(string token)
        {
            _editor.PutString(UserToken, token);
            _editor.Commit();
        }

        public string GetToken()
        {
            Init();
            return _preferences.GetString(UserToken, "");
        }
    }
}