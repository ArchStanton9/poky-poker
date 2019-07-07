using System;
using System.Net;
using System.Threading.Tasks;
using PokyPoker.Client.Credential;
using PokyPoker.Client.Serialization;
using PokyPoker.Contracts.Requests;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Topology;
using Vostok.Clusterclient.Transport.Webrequest;
using Vostok.Logging.Abstractions;

namespace PokyPoker.Client
{
    public class PokyPokerClient : IPokerClient
    {
        private readonly Uri url;
        private readonly ILog log = new SilentLog();
        private readonly ClusterClient client;

        public PokyPokerClient(Uri url)
        {
            this.url = url;
            client = new ClusterClient(log, Setup);

            Serializer = JsonBodySerializer.Default;
        }

        public ISerializer Serializer { get; set; }

        public IClientCredential ClientCredential { get; set; }

        private void Setup(IClusterClientConfiguration configuration)
        {
            configuration.ClusterProvider = new FixedClusterProvider(url);
            configuration.Transport = new WebRequestTransport(log);
            configuration.Logging.LogReplicaRequests = false;
            configuration.Logging.LogReplicaResults = false;
            configuration.Logging.LogRequestDetails = true;
            configuration.Logging.LogResultDetails = true;
        }

        #region Helpers

        private static ApiResponse BuildResponse(ClusterResult result)
        {
            var response = result.Response;
            var content = response.Content.ToArray();
            var statusCode = (int)response.Code;

            return new ApiResponse(content, (HttpStatusCode)statusCode);
        }

        private static ApiResponse<TContent> BuildResponse<TContent>(ClusterResult result, ISerializer serializer)
        {
            var response = result.Response;
            var content = response.Content.ToArray();
            var statusCode = (int)response.Code;

            return new ApiResponse<TContent>(content, serializer, (HttpStatusCode)statusCode);
        }

        #endregion

        public async Task<ApiResponse> CreateRoom(CreateRoomRequest createRoomRequest)
        {
            var request = Request.Post("api/rooms")
                .WithCredential(ClientCredential)
                .WithBody(createRoomRequest, Serializer);

            var result = await client.SendAsync(request).ConfigureAwait(false);
            return BuildResponse(result);
        }
    }
}
