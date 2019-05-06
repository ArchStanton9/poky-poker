using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PokyPoker.Domain.Exceptions
{
    public class GameLogicException : GameException
    {
        public GameLogicException()
        {
        }

        public GameLogicException(string message) : base(message)
        {
        }

        public GameLogicException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameLogicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
