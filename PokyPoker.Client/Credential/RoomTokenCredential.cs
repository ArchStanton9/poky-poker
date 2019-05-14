using System.Collections.Generic;

namespace PokyPoker.Client.Credential
{
    public class RoomTokenCredential : IClientCredential
    {
        private readonly KeyValuePair<string, string> value;

        public RoomTokenCredential(string token)
        {
            value = new KeyValuePair<string, string>("Authorization", $"room.token {token}");
        }

        public KeyValuePair<string, string> GetAuthHeader()
        {
            return value;
        }
    }
}