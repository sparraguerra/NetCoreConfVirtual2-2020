using Saint.Seiya.Shared.Models; 
using System;
using System.Text;

namespace Saint.Seiya.Domain.Helpers
{    
    public static class IdHelper
    {
        public readonly static string separator = "|";

        public static string GuidGenerator() 
        {                       
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="documentClassId"></param>
        /// <returns></returns>
        public static string PartitionKeyGenerator(string appId,string documentClassId) 
        {
            return appId + separator + documentClassId;
        }

        /// <summary>
        /// Recupera el AppId a partir del partitionKey
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public static string GetAppIdPartitionKey(string partitionKey)
        {
            if (!string.IsNullOrWhiteSpace(partitionKey))
            {
                string[] appIdSeparated = partitionKey.Split(separator);

                return appIdSeparated[0];
            }
            else 
            {
                throw new ArgumentNullException(nameof(partitionKey));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetSearchKey(SeiyaDocument doc)
        {            
            if (string.IsNullOrEmpty(doc.Id))                
            {
                doc.Id = IdHelper.GuidGenerator();
            }
            if (!string.IsNullOrWhiteSpace(doc.Id) && !string.IsNullOrWhiteSpace(doc.PartitionKey))
            {
                string key = $"{doc.Id}{separator}{doc.PartitionKey}";

                byte[] textAsBytes = Encoding.UTF8.GetBytes(key);
                return Convert.ToBase64String(textAsBytes);              
            }
            else         
            {
                throw new ArgumentNullException(nameof(doc));
            }
        }
    }
}
