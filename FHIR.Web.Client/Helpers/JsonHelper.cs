using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace FHIR.Web.Client.Helpers
{
    public static class JsonHelper
    {
        public static void ModifyProperties(JObject jsonObject)
        {
            foreach (var property in jsonObject.Properties())
            {
                if (property.Value is JObject valueObject && valueObject["value"] != null)
                {
                    property.Value = valueObject["value"];
                }
                else if (property.Value is JArray array)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        var item = array[i];
                        if (item is JObject itemObject && itemObject["value"] != null)
                        {
                            array[i] = itemObject["value"];

                        } else if(item is JObject)
                        {
                            ModifyProperties((JObject)item);
                        }
                    }
                }
                else if(property.Value is JObject)
                {
                    ModifyProperties((JObject)property.Value);
                }
            }
        }
    }

}
