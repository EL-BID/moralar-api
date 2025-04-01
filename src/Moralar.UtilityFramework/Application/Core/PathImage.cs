using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core
{
    public class PathImage : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return reader.Value?.ToString().Split('/').LastOrDefault();
            }
            catch (Exception)
            {
                return reader.Value;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                JToken.FromObject((value?.ToString()).SetPathImage()).WriteTo(writer);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
