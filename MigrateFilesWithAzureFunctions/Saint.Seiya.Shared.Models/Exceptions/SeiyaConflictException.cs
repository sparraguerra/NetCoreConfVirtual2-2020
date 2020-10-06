using System;
using System.Runtime.Serialization;

namespace Saint.Seiya.Shared.Models.Exceptions
{
    [Serializable]
    public class SeiyaConflictException : SeiyaBaseException
    {
        public SeiyaConflictException()
        {
        }

        public SeiyaConflictException(string message) : base(message)
        {
        }

        public SeiyaConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SeiyaConflictException(string message, string errorDetails) : base(message, errorDetails)
        {
        }

        public SeiyaConflictException(string message, string errorDetails, Exception innerException) : base(message, errorDetails, innerException)
        {
        }

        protected SeiyaConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
