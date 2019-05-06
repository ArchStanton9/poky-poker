using System;
using System.Runtime.Serialization;

namespace PokyPoker.Domain.Exceptions
{
    public class GameOrderException : GameException
    {
        public GameOrderException()
        {
        }

        public GameOrderException(string message) : base(message)
        {
        }

        public GameOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameOrderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
