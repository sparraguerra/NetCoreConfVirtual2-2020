using Saint.Ikki.Fx.Shared.Models;
using System;

namespace Saint.Ikki.Fx.Utils.Constants
{
    public class Queries
    {
        public string GetDocsById(string id)
        {
            return @"<View>" +
                     "<Query>" +
                       "<Where>" +
                           "<Eq>" +
                                 "<FieldRef Name='ID' />" +
                                 "<Value Type='Integer'>" + Convert.ToInt32(id) + "</Value>" +
                            "</Eq>" +
                       "</Where>" +
                     "</Query>" +
                   "</View>";
        }
         
        public string QueryDocuments(string rowLimit)
        {            
            return @"<View>" +
                    "<Query>" +
                    "<OrderBy>" +
                        "<FieldRef Name='Created' Ascending='false'></FieldRef>" +
                    "</OrderBy>" +
                    "<Where>" +
                        "<IsNull>" +
                                "<FieldRef Name='SeiyaStatus' />" +
                        "</IsNull>" + 
                    "</Where>" +
                    "</Query>" +
                    "<RowLimit>" + rowLimit + "</RowLimit>" +
                "</View>";
          
        }

        public string QueryDocumentsIkki()
        {
            return @"<View>" +
                    "<Query>" +
                    "<OrderBy>" +
                        "<FieldRef Name='Created' Ascending='false'></FieldRef>" +
                    "</OrderBy>" +
                    "</Query>" +
                "</View>";

        }
    }
}
