using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Velocity.Core.Extensions
{
    public static class SerializationExtensions
    {
        public static string Serialize(this object obj, Format format = Format.CamelCase)
        {
            var setttings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return format == Format.Normal
                           ? JsonConvert.SerializeObject(obj, Formatting.None, setttings)
                           : JsonConvert.SerializeObject(obj, Formatting.Indented, setttings);
        }

        public static TObject Deserialize<TObject>(this string serializedObject)
        {
            return JsonConvert.DeserializeObject<TObject>(serializedObject);
        }

        public static TObject DeserializeOrDefault<TObject>(this string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<TObject>(serializedObject);
            }
            catch
            {
                return default;
            }
        }
    }

    public enum Format
    {
        Normal,
        CamelCase
    }
}