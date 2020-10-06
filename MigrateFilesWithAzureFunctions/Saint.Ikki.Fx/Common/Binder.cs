using Microsoft.SharePoint.Client;
using Saint.Ikki.Fx.Shared.Models.Seiya.Metadatas;
using System;

namespace Saint.Ikki.Fx.Common
{
    public static class Metadata
    {
        public static MetadataDocument BindingTypes(ListItem metadata)
        {
            var metadatas = new MetadataDocument
            {
                Name = metadata["FileLeafRef"]?.ToString() ?? string.Empty,
                Title = metadata["Title"]?.ToString() ?? string.Empty, 
                FileType =  metadata["File_x0020_Type"]?.ToString() ?? string.Empty, 
                DocumentId = metadata["ID"]?.ToString() ?? string.Empty,
                Created = metadata["Created_x0020_Date"] != null ? Convert.ToDateTime(metadata["Created_x0020_Date"].ToString()) : (DateTime?)null,
                Modified = metadata["Last_x0020_Modified"] != null ? Convert.ToDateTime(metadata["Last_x0020_Modified"].ToString()) : (DateTime?)null, 
            };

            return metadatas;
        } 
    }
}
