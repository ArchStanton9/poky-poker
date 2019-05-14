using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokyPoker.Domain.Helpers;

namespace PokyPoker.WebApp.Auth
{
    public class RoomTokenAuthHandler : AuthenticationHandler<RoomAuthOptions>
    {
        private readonly ILogger logger;

        public RoomTokenAuthHandler(IOptionsMonitor<RoomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this.logger = logger.CreateLogger<RoomTokenAuthHandler>();
        }

        private static readonly AuthenticationProperties props = new AuthenticationProperties {IsPersistent = true};

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.User?.Identity.IsAuthenticated == true)
                return AuthenticateResult.NoResult();

            if (!TryReadToken(Request.Headers, out var token))
                return AuthenticateResult.Fail("No room token has been provided");

            try
            {
                var claims = GetPrincipalFromToken(token);
                if (claims == null)
                    return AuthenticateResult.NoResult();

                var identity = new ClaimsIdentity(claims, "Room Token");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, props, "room.token");

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error while retrieving principal from token {ex.Message}", ex);
                return AuthenticateResult.Fail("Error while retrieving principal.");
            }
        }

        public static bool TryReadToken(IHeaderDictionary headers, out string token)
        {
            if (headers.TryGetValue("Authorization", out var strValue))
            {
                var authHeader = strValue.ToString();
                if (authHeader.StartsWith("room.token"))
                {
                    token = authHeader.Substring(10);
                    return true;
                }
            }

            token = default;
            return false;
        }

        private static IEnumerable<Claim> GetPrincipalFromToken(string token)
        {
            var bytes = Convert.FromBase64String(token);

            var roomId = new Guid(bytes.Slice(0, 16));
            var userId = new Guid(bytes.Slice(16, 16));

            yield return new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString());
            yield return new Claim("roomId", roomId.ToString());
        }
    }
}
