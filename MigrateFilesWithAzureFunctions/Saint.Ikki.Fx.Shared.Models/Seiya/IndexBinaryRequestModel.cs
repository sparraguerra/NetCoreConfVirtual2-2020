using Saint.Ikki.Fx.Shared.Models.Seiya.Extensions; 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Saint.Ikki.Fx.Shared.Models.Seiya
{
    
    public class IndexBinaryRequestModel : BaseModel
    {
        [Required]
        public string DocumentClassId { get; set; }
        [Required]
        public string DocumentClass { get; set; }
        [Required]
        public string DocumentType { get; set; }
        public JObject Properties { get; set; }
        public string GetCreatedUser() => User;
        public string GetId() => string.Empty;
        public string GetLastModifiedUser() => User;
        public DateTime GetCreated() => DateTime.UtcNow;
        public DateTime GetModified() => GetCreated();

        public void MapProperties<T>(T model) where T : class
        {
            if (Properties == null)
            {
                Properties = new JObject();
            }

            var metadataProperties = model.GetType().GetMetadataProperties();

            foreach (var property in metadataProperties)
            {
                if (property.PropertyType.IsAssignableFrom(typeof(Stream)))
                {
                    continue;
                }

                try
                {
                    var propertyValue = property.GetValue(model)?.ToString();
                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        var metadataObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(property.GetValue(model).ToString());
                        foreach (var metadataObjectProp in metadataObject)
                        {
                            Properties.Add(metadataObjectProp.Key, JToken.FromObject(metadataObjectProp.Value));
                        }
                    }
                }
                catch (Exception)
                {
                    Properties.Add(property.Name, JToken.FromObject(property.GetValue(model) ?? string.Empty));
                }
            }
        } 
    }
}
