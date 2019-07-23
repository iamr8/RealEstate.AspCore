using System;
using RealEstate.Base;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.KavenNegarProvider.Response.ResultModels
{
    public class Receive
    {
        public long Date { get; set; }

        public DateTime GregorianDate => Date.ToDateTime();

        public long MessageId { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }
    }
}