using System;
using System.Runtime.Serialization;

namespace Saint.Seiya.Shared.Models.Exceptions
{
    [Serializable]
    public class SeiyaDataAccessException : SeiyaBaseException
    {
        public SeiyaDataAccessException()
        {
        }

        public SeiyaDataAccessException(string message) : base(message)
        {
        }

        public SeiyaDataAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SeiyaDataAccessException(string documentId, string message, Exception innerException) : base(message, AddInfo(message, documentId), innerException)
        {
        }

        protected SeiyaDataAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string AddInfo(string message, string documentId)
        {
            return $"{message}. Document Id: {documentId}";
        }
    }
}
