using System;
using System.Runtime.Serialization;

namespace PokyPoker.Domain.Exceptions
{
    public class GameStateException : GameException
    {
        public GameStateException()
        {
        }

        public GameStateException(string message) : base(message)
        {
        }

        public GameStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
