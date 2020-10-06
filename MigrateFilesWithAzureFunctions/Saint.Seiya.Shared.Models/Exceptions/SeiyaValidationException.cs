using System;
using System.Runtime.Serialization;

namespace Saint.Seiya.Shared.Models.Exceptions
{
    [Serializable]
    public class SeiyaValidationException : SeiyaBaseException
    {
        public SeiyaValidationException()
        {
        }

        public SeiyaValidationException(string message) : base(message)
        {
        } 

        public SeiyaValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SeiyaValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
 
    }
}
