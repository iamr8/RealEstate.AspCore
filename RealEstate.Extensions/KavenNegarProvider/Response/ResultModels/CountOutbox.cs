namespace RealEstate.Extensions.KavenNegarProvider.Response.ResultModels
{
    public class CountOutbox : CountInbox
    {
        public long SumPart { get; set; }
        public long Cost { get; set; }
    }
}