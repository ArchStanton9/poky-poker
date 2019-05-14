using System;
using Microsoft.AspNetCore.Authentication;

namespace PokyPoker.WebApp.Auth
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddRoomTokenAuth(this AuthenticationBuilder builder,
            Action<RoomAuthOptions> configureOptions)
        {
            return builder.AddScheme<RoomAuthOptions, RoomTokenAuthHandler>("room.token", "Room Token Auth",
                configureOptions);
        }
    }
}
