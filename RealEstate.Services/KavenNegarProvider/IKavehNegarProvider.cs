using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RealEstate.Services.KavenNegarProvider.Enums;
using RealEstate.Services.KavenNegarProvider.Response;
using RealEstate.Services.KavenNegarProvider.Response.ResultModels;
using RealEstate.Services.KavenNegarProvider.Utils;
using RestSharp;

namespace RealEstate.Services.KavenNegarProvider
{
    public interface IKavehNegarProvider
    {
        Response<List<Send>> Send(List<string> receptor, string message);

        Response<Send> Send(string receptor, string message);

        Response<Send> Send(string receptor, string message, MessageType type, DateTime date);

        Response<Send> Send(string receptor, string message, MessageType type, DateTime date,
            string localId);

        Response<Send> Send(string receptor, string message, string localId);

        Response<List<Send>> Send(List<string> receptors, string message, string localId);

        Response<List<Send>> SendSimultaneously(List<string> receptor, string message, MessageType type, DateTime date,
            List<string> localIds = null);

        Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages);

        Response<List<Send>> SendArray(List<string> receptors, List<string> messages, MessageType type,
            DateTime date);

        Response<List<Send>> SendArray(List<string> receptors, List<string> messages, MessageType type,
            DateTime date, string localMessageIds);

        Response<List<Send>> SendArray(List<string> receptors, List<string> messages, string localMessageId);

        Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages,
            string localMessageId);

        Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages,
            List<MessageType> types, DateTime date, List<string> localMessageIds);

        List<Status> Status(List<string> messageIds);

        Status Status(string messageId);

        List<LocalMessageId> StatusLocalMessageId(List<string> messageIds);

        LocalMessageId StatusLocalMessageId(string messageId);

        List<Send> Select(List<string> messageIds);

        Send Select(string messageId);

        List<Send> SelectOutbox(DateTime startDate);

        List<Send> SelectOutbox(DateTime startDate, DateTime endDate);

        List<Send> SelectOutbox(DateTime startDate, DateTime endDate, String sender);

        List<Send> LatestOutbox(long pageSize);

        List<Send> LatestOutbox(long pageSize, String sender);

        CountOutbox CountOutbox(DateTime startDate);

        CountOutbox CountOutbox(DateTime startDate, DateTime endDate);

        CountOutbox CountOutbox(DateTime startDate, DateTime endDate, int status);

        List<Status> Cancel(List<string> ids);

        Status Cancel(string messageId);

        List<Receive> Receive(string line, int isRead);

        CountInbox CountInbox(DateTime startDate, string lineNumber);

        CountInbox CountInbox(DateTime startDate, DateTime endDate, string lineNumber);

        CountInbox CountInbox(DateTime startDate, DateTime endDate, string lineNumber, int isRead);

        Response<List<CountPostalCode>> CountPostalCode(long postalCode);

        Response<List<Send>> SendByPostalCode(long postalcode, string message, long mciStartIndex,
            long mciCount, long mtnStartIndex, long mtnCount);

        Response<List<Send>> SendByPostalCode(long postalcode, string message, long mciStartIndex,
            long mciCount, long mtnStartIndex, long mtnCount, DateTime date);

        AccountInfo AccountInfo();

        Response<AccountConfig> AccountConfig(string apiLogs, string dailyReport, string debugMode, int? minCreditAlarm, string resendFailed);

        Response<List<Send>> VerifyLookup(string receptor, string token, string template);

        Response<List<Send>> VerifyLookup(string receptor, string token, string template, VerifyLookupType type);

        Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string template);

        Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string template);

        Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string template,
            VerifyLookupType type);

        Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string template, VerifyLookupType type);

        Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string token20, string template, VerifyLookupType type);

        Response<List<Send>> CallMakeTTS(string message, string receptor);

        Response<List<Send>> CallMakeTTS(string message, List<string> receptor);

        Response<List<Send>> CallMakeTTS(string message, List<string> receptor, DateTime? date, List<string> localId);
    }

    public class KavehNegarProvider : IKavehNegarProvider
    {
        private const string ApiPath = "https://api.kavenegar.com/v1/{0}/{1}/{2}.{3}";
        private const string SmsNumber = "10002020505050";
        private const string ApiKey = "7759476A5352684A4D653867304B4E61376A393851534F46567375506445366B";

        private string GetApiPath(string _base, string method, string output)
        {
            return string.Format(ApiPath, ApiKey, _base, method, output);
        }

        private static string Execute(string path, Dictionary<string, object> _params)
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
            return response.Content;
        }

        public Response<List<Send>> Send(List<string> receptor, string message)
        {
            return SendSimultaneously(receptor, message, MessageType.MobileMemory, DateTime.MinValue);
        }

        public Response<Send> Send(string receptor, string message)
        {
            return Send(receptor, message, MessageType.MobileMemory, DateTime.MinValue);
        }

        public Response<Send> Send(string receptor, string message, MessageType type, DateTime date)
        {
            return Send(receptor, message, type, date, null);
        }

        public Response<Send> Send(string receptor, string message, MessageType type, DateTime date, string localId)
        {
            var receptors = new List<string> { receptor };
            var localIds = new List<string> { localId };
            var sendStatus = SendSimultaneously(receptors, message, MessageType.MobileMemory, DateTime.MinValue, localIds);
            var temp = new Response<Send>
            {
                Result = sendStatus.Result[0],
                RequestStatus = sendStatus.RequestStatus
            };
            return temp;
        }

        public Response<Send> Send(string receptor, string message, string localId)
        {
            return Send(receptor, message, MessageType.MobileMemory, DateTime.MinValue, localId);
        }

        public Response<List<Send>> Send(List<string> receptors, string message, string localId)
        {
            var localIds = new List<string>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                localIds.Add(localId);
            }
            return SendSimultaneously(receptors, message, MessageType.MobileMemory, DateTime.MinValue, localIds);
        }

        public Response<List<Send>> SendSimultaneously(List<string> receptor, string message, MessageType type,
            DateTime date, List<string> localIds = null)
        {
            var path = GetApiPath("sms", "send", "json");
            var param = new Dictionary<string, object>
            {
                {"sender", System.Web.HttpUtility.UrlEncode(SmsNumber)},
                {"receptor", System.Web.HttpUtility.UrlEncode(string.Join(",", receptor.ToArray()))},
                {"message", System.Web.HttpUtility.UrlEncode(message)},
                {"type", (int) type},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };
            if (localIds?.Count > 0)
            {
                param.Add("localid", string.Join(",", localIds.ToArray()));
            }

            var responseBody = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(responseBody);
            return deserialize;
        }

        public Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages)
        {
            var types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }

            return SendArray(senders, receptors, messages, types, DateTime.MinValue, null);
        }

        public Response<List<Send>> SendArray(List<string> receptors, List<string> messages,
            MessageType type, DateTime date)
        {
            var senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(SmsNumber);
            }

            var types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }

            return SendArray(senders, receptors, messages, types, date, null);
        }

        public Response<List<Send>> SendArray(List<string> receptors, List<string> messages,
            MessageType type, DateTime date, string localMessageIds)
        {
            var senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(SmsNumber);
            }

            var types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }

            return SendArray(senders, receptors, messages, types, date, new List<string>() { localMessageIds });
        }

        public Response<List<Send>> SendArray(List<string> receptors, List<string> messages, string localMessageId)
        {
            var senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(SmsNumber);
            }

            return SendArray(senders, receptors, messages, localMessageId);
        }

        public Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages, string localMessageId)
        {
            var types = new List<MessageType>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }
            var localMessageIds = new List<string>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                localMessageIds.Add(localMessageId);
            }
            return SendArray(senders, receptors, messages, types, DateTime.MinValue, localMessageIds);
        }

        public Response<List<Send>> SendArray(List<string> senders, List<string> receptors, List<string> messages,
            List<MessageType> types, DateTime date, List<string> localMessageIds)
        {
            var path = GetApiPath("sms", "sendarray", "json");
            var jsonSenders = JsonConvert.SerializeObject(senders);
            var jsonReceptors = JsonConvert.SerializeObject(receptors);
            var jsonMessages = JsonConvert.SerializeObject(messages);
            var jsonTypes = JsonConvert.SerializeObject(types);
            var param = new Dictionary<string, object>
            {
                {"message", jsonMessages},
                {"sender", jsonSenders},
                {"receptor", jsonReceptors},
                {"type", jsonTypes},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };

            if (localMessageIds?.Count > 0)
                param.Add("localmessageids", string.Join(",", localMessageIds.ToArray()));

            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize;
        }

        public List<Status> Status(List<string> messageIds)
        {
            var path = GetApiPath("sms", "status", "json");
            var param = new Dictionary<string, object>
            {
                {"messageid", string.Join(",", messageIds.ToArray())}
            };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Status>>>(response);
            return deserialize.Result;
        }

        public Status Status(string messageId)
        {
            var ids = new List<string> { messageId };
            var result = Status(ids);
            return result.Count == 1 ? result[0] : null;
        }

        public List<LocalMessageId> StatusLocalMessageId(List<string> messageIds)
        {
            var path = GetApiPath("sms", "statuslocalmessageid", "json");
            var param = new Dictionary<string, object> { { "localid", string.Join(",", messageIds.ToArray()) } };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<LocalMessageId>>>(response);
            return deserialize.Result;
        }

        public LocalMessageId StatusLocalMessageId(string messageId)
        {
            var result = StatusLocalMessageId(new List<String>() { messageId });
            return result.Count == 1 ? result[0] : null;
        }

        public List<Send> Select(List<string> messageIds)
        {
            var path = GetApiPath("sms", "select", "json");
            var param = new Dictionary<string, object> { { "messageid", string.Join(",", messageIds.ToArray()) } };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize.Result;
        }

        public Send Select(string messageId)
        {
            var ids = new List<string> { messageId };
            var result = Select(ids);
            return result.Count == 1 ? result[0] : null;
        }

        public List<Send> SelectOutbox(DateTime startDate)
        {
            return SelectOutbox(startDate, DateTime.MaxValue);
        }

        public List<Send> SelectOutbox(DateTime startDate, DateTime endDate)
        {
            return SelectOutbox(startDate, endDate, null);
        }

        public List<Send> SelectOutbox(DateTime startDate, DateTime endDate, String sender)
        {
            var path = GetApiPath("sms", "selectoutbox", "json");
            var param = new Dictionary<string, object>
         {
             {"startdate", startDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startDate)},
             {"enddate", endDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(endDate)},
             {"sender", sender}
         };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize.Result;
        }

        public List<Send> LatestOutbox(long pageSize)
        {
            return LatestOutbox(pageSize, "");
        }

        public List<Send> LatestOutbox(long pageSize, string sender)
        {
            var path = GetApiPath("sms", "latestoutbox", "json");
            var param = new Dictionary<string, object> { { "pagesize", pageSize }, { "sender", sender } };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize.Result;
        }

        public CountOutbox CountOutbox(DateTime startDate)
        {
            return CountOutbox(startDate, DateTime.MaxValue, 10);
        }

        public CountOutbox CountOutbox(DateTime startDate, DateTime endDate)
        {
            return CountOutbox(startDate, endDate, 0);
        }

        public CountOutbox CountOutbox(DateTime startDate, DateTime endDate, int status)
        {
            var path = GetApiPath("sms", "countoutbox", "json");
            var param = new Dictionary<string, object>
         {
             {"startdate", startDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startDate)},
             {"enddate", endDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(endDate)},
             {"status", status}
         };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<CountOutbox>>>(response);
            return deserialize.Result?[0] ?? new CountOutbox();
        }

        public List<Status> Cancel(List<string> ids)
        {
            var path = GetApiPath("sms", "cancel", "json");
            var param = new Dictionary<string, object>
            {
                {"messageid", string.Join(",", ids.ToArray())}
            };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Status>>>(response);
            return deserialize.Result;
        }

        public Status Cancel(string messageId)
        {
            var ids = new List<string> { messageId };
            var result = Cancel(ids);
            return result.Count == 1 ? result[0] : null;
        }

        public List<Receive> Receive(string line, int isRead)
        {
            var path = GetApiPath("sms", "receive", "json");
            var param = new Dictionary<string, object> { { "linenumber", line }, { "isread", isRead } };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Receive>>>(response);
            return deserialize.Result;
        }

        public CountInbox CountInbox(DateTime startDate, string lineNumber)
        {
            return CountInbox(startDate, DateTime.MaxValue, lineNumber, 0);
        }

        public CountInbox CountInbox(DateTime startDate, DateTime endDate, string lineNumber)
        {
            return CountInbox(startDate, endDate, lineNumber, 0);
        }

        public CountInbox CountInbox(DateTime startDate, DateTime endDate, string lineNumber, int isRead)
        {
            var path = GetApiPath("sms", "countoutbox", "json");
            var param = new Dictionary<string, object>
        {
            {"startdate", startDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startDate)},
            {"enddate", endDate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(endDate)},
            {"linenumber", lineNumber},
            {"isread", isRead}
        };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<CountInbox>>>(response);
            return deserialize?.Result[0] ?? new CountInbox();
        }

        public Response<List<CountPostalCode>> CountPostalCode(long postalCode)
        {
            var path = GetApiPath("sms", "countpostalcode", "json");
            var param = new Dictionary<string, object> { { "postalcode", postalCode } };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<CountPostalCode>>>(response);
            return deserialize;
        }

        public Response<List<Send>> SendByPostalCode(long postalcode, string message, long mciStartIndex,
            long mciCount, long mtnStartIndex, long mtnCount)
        {
            return SendByPostalCode(postalcode, message, mciStartIndex, mciCount, mtnStartIndex, mtnCount,
                DateTime.MinValue);
        }

        public Response<List<Send>> SendByPostalCode(long postalcode, string message, long mciStartIndex,
            long mciCount, long mtnStartIndex, long mtnCount, DateTime date)
        {
            var path = GetApiPath("sms", "sendbypostalcode", "json");
            var param = new Dictionary<string, object>
            {
                {"postalcode", postalcode},
                {"sender", SmsNumber},
                {"message", System.Web.HttpUtility.UrlEncode(message)},
                {"mcistartIndex", mciStartIndex},
                {"mcicount", mciCount},
                {"mtnstartindex", mtnStartIndex},
                {"mtncount", mtnCount},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize;
        }

        public AccountInfo AccountInfo()
        {
            var path = GetApiPath("account", "info", "json");
            var response = Execute(path, null);
            var deserialize = JsonConvert.DeserializeObject<Response<AccountInfo>>(response);
            return deserialize.Result;
        }

        public Response<AccountConfig> AccountConfig(string apiLogs, string dailyReport, string debugMode, int? minCreditAlarm, string resendFailed)
        {
            var path = GetApiPath("account", "config", "json");
            var param = new Dictionary<string, object>
            {
                {"apilogs", apiLogs},
                {"dailyreport", dailyReport},
                {"debugmode", debugMode},
                {"defaultsender", SmsNumber},
                {"mincreditalarm", minCreditAlarm},
                {"resendfailed", resendFailed}
            };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<AccountConfig>>(response);
            return deserialize;
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string template)
        {
            return VerifyLookup(receptor, token, null, null, template, VerifyLookupType.Sms);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string template, VerifyLookupType type)
        {
            return VerifyLookup(receptor, token, null, null, template, type);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string template)
        {
            return VerifyLookup(receptor, token, token2, token3, template, VerifyLookupType.Sms);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string template)
        {
            return VerifyLookup(receptor, token, token2, token3, token10, template, VerifyLookupType.Sms);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string template,
            VerifyLookupType type)
        {
            return VerifyLookup(receptor, token, token2, token3, null, template, type);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string template, VerifyLookupType type)
        {
            return VerifyLookup(receptor, token, token2, token3, token10, null, template, type);
        }

        public Response<List<Send>> VerifyLookup(string receptor, string token, string token2, string token3, string token10,
            string token20, string template, VerifyLookupType type)
        {
            var path = GetApiPath("verify", "lookup", "json");
            var param = new Dictionary<string, object>
            {
                {"receptor", receptor},
                {"template", template},
                {"token", token},
                {"token2", token2},
                {"token3", token3},
                {"token10", token10},
                {"token20", token20},
                {"type", type},
            };
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize;
        }

        #region << CallMakeTTS >>

        public Response<List<Send>> CallMakeTTS(string message, string receptor)
        {
            return CallMakeTTS(message, new List<string> { receptor }, null, null);
        }

        public Response<List<Send>> CallMakeTTS(string message, List<string> receptor)
        {
            return CallMakeTTS(message, receptor, null, null);
        }

        public Response<List<Send>> CallMakeTTS(string message, List<string> receptor, DateTime? date, List<string> localId)
        {
            var path = GetApiPath("call", "maketts", "json");
            var param = new Dictionary<string, object>
            {
                {"receptor", string.Join(",", receptor.ToArray())},
                {"message", System.Web.HttpUtility.UrlEncode(message)},
            };
            if (date != null)
                param.Add("date", DateHelper.DateTimeToUnixTimestamp(date.Value));
            if (localId?.Count > 0)
                param.Add("localid", string.Join(",", localId.ToArray()));
            var response = Execute(path, param);
            var deserialize = JsonConvert.DeserializeObject<Response<List<Send>>>(response);
            return deserialize;
        }

        #endregion << CallMakeTTS >>
    }
}