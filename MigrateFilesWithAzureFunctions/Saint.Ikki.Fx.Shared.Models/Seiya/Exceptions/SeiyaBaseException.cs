using System;
using System.Runtime.Serialization;

namespace Saint.Ikki.Fx.Shared.Models.Seiya.Exceptions
{
    [Serializable]
    public class SeiyaBaseException : Exception
    { 
        public object ErrorObject { get; set; }

        public string ErrorDetails { get; set; }

        public string ErrorCode { get; set; }

        public SeiyaBaseException()
        {
        }

        public SeiyaBaseException(string message) : base(message)
        {
        }

        public SeiyaBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SeiyaBaseException(string message, string errorDetails, Exception innerException) : base(message, innerException)
        {
            this.ErrorDetails = errorDetails;
        }

        public SeiyaBaseException(string message, string errorDetails) : base(message)
        {
            this.ErrorDetails = errorDetails;
        }

        public SeiyaBaseException(object errorObject)
        {
            ErrorObject = errorObject;
        }

        protected SeiyaBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
