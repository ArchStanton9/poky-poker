using Vostok.Clusterclient.Core.Model;

namespace PokyPoker.Client.Serialization
{
    public static class RequestContentExtensions
    {
        public static Request WithBody<TContent>(this Request request, TContent body, ISerializer serializer)
        {
            var content = new Content(serializer.Serialize(body));
            return request.WithContent(content);
        }
    }
}
