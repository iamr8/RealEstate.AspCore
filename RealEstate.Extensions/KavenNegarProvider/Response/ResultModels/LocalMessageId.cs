using RealEstate.Extensions.KavenNegarProvider.Enums;

namespace RealEstate.Extensions.KavenNegarProvider.Response.ResultModels
{
    public class LocalMessageId
    {
        public long MessageId { get; set; }
        public long LocalId { get; set; }
        public MessageStatus Status { get; set; }
        public string StatusText { get; set; }
    }
}