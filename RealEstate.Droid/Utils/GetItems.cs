using Newtonsoft.Json;
using RestSharp;
using SQLite;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Droid.Utils
{
    public static class GetItems
    {
        public static async Task<TModel> CreateRequest<TModel, TPayload>(Methods actionUrl, TPayload payload) where TModel : class, new()
        {
            var baseUrl = PreferencesHelper.GetInstance().GetBaseUrl();
            var url = actionUrl.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (string.IsNullOrEmpty(url))
                return default;

            var path = Path.Combine(baseUrl, url);
            var client = new RestClient(path);

            var jsonPayload = JsonConvert.SerializeObject(payload);

            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(jsonPayload);
            var response = await client.ExecuteTaskAsync<TModel>(request);

            var content = response.Data;
            return content;
        }

        public static void CreateDatabase()
        {
            const string dbName = "rsdb.db3";
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbName);
            var db = new SQLiteConnection(dbPath);
            //db.CreateTable()
        }
    }
}