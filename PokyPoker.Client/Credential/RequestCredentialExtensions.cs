using Vostok.Clusterclient.Core.Model;

namespace PokyPoker.Client.Credential
{
    public static class RequestCredentialExtensions
    {
        public static Request WithCredential(this Request request, IClientCredential credential)
        {
            if (credential == null)
                return request;

            var authHeader = credential.GetAuthHeader();
            return request.WithHeader(authHeader.Key, authHeader.Value);
        }
    }
}
