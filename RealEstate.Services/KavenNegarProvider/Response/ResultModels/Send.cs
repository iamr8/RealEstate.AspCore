using RealEstate.Base;
using System;

namespace RealEstate.Services.KavenNegarProvider.Response.ResultModels
{
    public class Send
    {
        public long MessageId { get; set; }

        public int Cost { get; set; }

        public DateTime GregorianDate
        {
            get => Date.ToDateTime();
            set => Date = value.ToUnixTimestamp();
        }

        public long Date { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }

        public string Sender { get; set; }
        public int Status { get; set; }

        public string StatusText { get; set; }
    }
}