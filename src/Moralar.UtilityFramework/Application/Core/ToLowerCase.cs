using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Moralar.UtilityFramework.Application.Core
{
    public class ToLowerCase : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return reader.Value?.ToString().ToLower();
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
                JToken.FromObject(value?.ToString().ToLower()).WriteTo(writer);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
