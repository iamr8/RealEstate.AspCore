using System;
using RealEstate.Base;

namespace RealEstate.Services.KavenNegarProvider.Response.ResultModels
{
    public class AccountInfo
    {
        public long RemainCredit { get; set; }
        public long ExpireDate { get; set; }

        public DateTime GregorianExpireDate => ExpireDate.ToDateTime();

        public string Type { get; set; }
    }
}