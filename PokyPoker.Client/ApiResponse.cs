using System;
using System.Net;
using PokyPoker.Client.Serialization;

namespace PokyPoker.Client
{
    public class ApiResponse
    {
        public ApiResponse(byte[] body, HttpStatusCode code)
        {
            StatusCode = code;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; }

        public byte[] Body { get; }

        public bool IsSuccessful => (short) StatusCode >= 200 && (short) StatusCode < 300;
    }

    public class ApiResponse<TContent> : ApiResponse
    {
        private readonly ISerializer serializer;
        private readonly Lazy<TContent> lazyContent;

        public ApiResponse(byte[] body, ISerializer serializer, HttpStatusCode code) : base(body, code)
        {
            this.serializer = serializer;
            lazyContent = new Lazy<TContent>(() => serializer.Deserialize<TContent>(body));
        }

        public TContent Content => lazyContent.Value;
    }
}
