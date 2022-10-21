using System;
using System.Runtime.Serialization;

namespace JdaTools.Connection
{
    [Serializable]
    internal class MocaException : Exception
    {
        public MocaResponse Response { get; private set; }

        public MocaException()
        {
        }

        public MocaException(string message) : base(message)
        {
        }

        public MocaException(MocaResponse response) : base(response.status + ": " + response.message)
        {
            this.Response = response;
        }

        public MocaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MocaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}