using System.Collections.Generic;

namespace PokyPoker.Client.Credential
{
    public interface IClientCredential
    {
        KeyValuePair<string, string> GetAuthHeader();
    }
}
