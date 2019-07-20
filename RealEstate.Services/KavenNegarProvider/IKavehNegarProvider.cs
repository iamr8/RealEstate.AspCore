using Newtonsoft.Json;
using RealEstate.Services.KavenNegarProvider.Enums;
using RealEstate.Services.KavenNegarProvider.Response;
using RealEstate.Services.KavenNegarProvider.Response.ResultModels;
using RealEstate.Services.KavenNegarProvider.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstate.Services.KavenNegarProvider
{
    public interface IKavehNegarProvider
    {
        Response<List<Send>> Send(List<string> receptors, string message, DateTime? scheduledDateTime = null);

        Response<AccountConfig> AccountConfig(string apiLogs, string dailyReport, string debugMode, int? minCreditAlarm, string resendFailed);

        List<Send> Select(params string[] messageIds);

        Response<List<Status>> Status(params string[] messageIds);

        Response<List<Send>> LatestOutbox(long pageSize, string sender = null);

        Response<List<Status>> Cancel(params string[] messageIds);

        Response<AccountInfo> AccountInfo();

        Response<List<Send>> VerifyLookup(string receptor, string template, string token, string token2 = null, string token3 = null, string token10 = null,
            string token20 = null);
    }

    public class KavehNegarProvider : IKavehNegarProvider
    {
        private const string SmsNumber = "10002020505050";
        private const string ApiKey = "7759476A5352684A4D653867304B4E61376A393851534F46567375506445366B";

        private static string GetApiPath(string _base, string method)
        {
            if (_base == null)
                throw new ArgumentNullException(nameof(_base));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return $"https://api.kavenegar.com/v1/{ApiKey}/{_base}/{method}.json";
        }

        private static TResponse Execute<TResponse>(string path, Dictionary<string, object> _params) where TResponse : class
        {
            var postData = "";
            postData = _params.Keys.Aggregate(postData,
                (current, key) => current + $"{key}={_params[key]}&");

            var client = new RestClient(path);
            var request = new RestRequest(Method.POST)
            {
                Timeout = -1
            };
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", postData, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (response == null)
                return default;

            var objJson = JsonConvert.DeserializeObject<TResponse>(response.Content);
            return objJson;
        }

        public Response<List<Send>> Send(List<string> receptors, string message, DateTime? scheduledDateTime = null)
        {
            var param = new Dictionary<string, object>
            {
                {"sender", HttpUtility.UrlEncode(SmsNumber)},
                {"receptor", HttpUtility.UrlEncode(string.Join(",", receptors.ToArray()))},
                {"message", HttpUtility.UrlEncode(message)},
                {"type", (int)MessageType.MobileMemory},
                {"date", scheduledDateTime == null ? 0 : DateHelper.DateTimeToUnixTimestamp((DateTime)scheduledDateTime)}
            };

            var path = GetApiPath("sms", "send");
            var response = Execute<Response<List<Send>>>(path, param);
            return response;
        }

        public Response<AccountConfig> AccountConfig(string apiLogs, string dailyReport, string debugMode, int? minCreditAlarm, string resendFailed)
        {
            var param = new Dictionary<string, object>
            {
                {"apilogs", apiLogs},
                {"dailyreport", dailyReport},
                {"debugmode", debugMode},
                {"defaultsender", SmsNumber},
                {"mincreditalarm", minCreditAlarm},
                {"resendfailed", resendFailed}
            };

            var path = GetApiPath("account", "config");
            var response = Execute<Response<AccountConfig>>(path, param);
            return response;
        }

        public List<Send> Select(params string[] messageIds)
        {
            var param = new Dictionary<string, object>
            {
                { "messageid", string.Join(",", messageIds.ToArray()) }
            };

            var path = GetApiPath("sms", "select");
            var response = Execute<Response<List<Send>>>(path, param);
            return response.Result;
        }

        public Response<List<Status>> Status(params string[] messageIds)
        {
            var param = new Dictionary<string, object>
            {
                {"messageid", string.Join(",", messageIds.ToArray())}
            };

            var path = GetApiPath("sms", "status");
            var response = Execute<Response<List<Status>>>(path, param);
            return response;
        }

        public Response<List<Send>> LatestOutbox(long pageSize, string sender = null)
        {
            var param = new Dictionary<string, object>
            {
                { "pagesize", pageSize },
                { "sender", sender }
            };

            var path = GetApiPath("sms", "latestoutbox");
            var response = Execute<Response<List<Send>>>(path, param);
            return response;
        }

        public Response<List<Status>> Cancel(params string[] messageIds)
        {
            var param = new Dictionary<string, object>
            {
                {"messageid", string.Join(",", messageIds.ToArray())}
            };

            var path = GetApiPath("sms", "cancel");
            var response = Execute<Response<List<Status>>>(path, param);
            return response;
        }

        public Response<AccountInfo> AccountInfo()
        {
            var path = GetApiPath("account", "info");
            var response = Execute<Response<AccountInfo>>(path, null);
            return response;
        }

        public Response<List<Send>> VerifyLookup(string receptor, string template, string token, string token2 = null, string token3 = null, string token10 = null, string token20 = null)
        {
            var param = new Dictionary<string, object>
            {
                {"receptor", receptor},
                {"template", template},
                {"token", token},
                {"token2", token2},
                {"token3", token3},
                {"token10", token10},
                {"token20", token20},
                {"type", VerifyLookupType.Sms},
            };

            var path = GetApiPath("verify", "lookup");
            var response = Execute<Response<List<Send>>>(path, param);
            return response;
        }
    }
}