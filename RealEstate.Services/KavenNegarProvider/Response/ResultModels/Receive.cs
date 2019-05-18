using System;
using RealEstate.Services.KavenNegarProvider.Utils;

namespace RealEstate.Services.KavenNegarProvider.Response.ResultModels
{
    public class Receive
    {
        public long Date { get; set; }

        public DateTime GregorianDate => DateHelper.UnixTimestampToDateTime(Date);

        public long MessageId { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }
    }
}