using System;
using RealEstate.Services.KavenNegarProvider.Utils;

namespace RealEstate.Services.KavenNegarProvider.Response.ResultModels
{
    public class AccountInfo
    {
        public long RemainCredit { get; set; }
        public long ExpireDate { get; set; }

        public DateTime GregorianExpireDate => DateHelper.UnixTimestampToDateTime(ExpireDate);

        public string Type { get; set; }
    }
}