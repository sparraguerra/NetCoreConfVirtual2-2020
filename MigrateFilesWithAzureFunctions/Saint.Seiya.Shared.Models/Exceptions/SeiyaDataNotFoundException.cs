using System;
using System.Runtime.Serialization;

namespace Saint.Seiya.Shared.Models.Exceptions
{
    [Serializable]
    public class SeiyaDataNotFoundException : SeiyaBaseException
    {
        public SeiyaDataNotFoundException()
        {
        }

        public SeiyaDataNotFoundException(string message) : base(message)
        {
        }

        public SeiyaDataNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SeiyaDataNotFoundException(string documentId, string message, Exception innerException) : base(message, AddInfo(message, documentId), innerException)
        {
        }

        protected SeiyaDataNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string AddInfo(string message, string documentId)
        {
            return $"{message}. Document Id: {documentId}";
        }
    }
}
