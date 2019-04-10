using System;
using RealEstate.Services.Extensions.KavenNegarProvider.Utils;

namespace RealEstate.Services.Extensions.KavenNegarProvider.Response.ResultModels
{
    public class Send
    {
        public long MessageId { get; set; }

        public int Cost { get; set; }

        public DateTime GregorianDate
        {
            get => DateHelper.UnixTimestampToDateTime(Date);
            set => Date = DateHelper.DateTimeToUnixTimestamp(value);
        }

        public long Date { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }

        public string Sender { get; set; }
        public int Status { get; set; }

        public string StatusText { get; set; }
    }
}