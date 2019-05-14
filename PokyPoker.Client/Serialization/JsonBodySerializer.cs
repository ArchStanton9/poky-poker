using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PokyPoker.Client.Serialization
{
    public class JsonBodySerializer : ISerializer
    {
        private readonly JsonSerializerSettings settings;
        private readonly JsonSerializer bodySerializer;
        private readonly Encoding encoding = Encoding.UTF8;

        public JsonBodySerializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
            bodySerializer = JsonSerializer.Create(settings);
        }

        public string ContentType { get; } = "application/json";

        public void Serialize(object item, Stream outputStream)
        {
            using (var writer = new StreamWriter(outputStream, encoding))
            {
                bodySerializer.Serialize(writer, item);
            }
        }

        public byte[] Serialize(object item)
        {
            return encoding.GetBytes(JsonConvert.SerializeObject(item, settings));
        }

        public T Deserialize<T>(Stream inputStream)
        {
            using (var reader = new StreamReader(inputStream, encoding))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return bodySerializer.Deserialize<T>(jsonReader);
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var json = encoding.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public bool TryDeserialize<TValue>(Stream inputStream, out TValue value)
        {
            try
            {
                value = Deserialize<TValue>(inputStream);
                return true;
            }
            catch (JsonException)
            {
                value = default;
                return false;
            }
        }

        public bool TryDeserialize<TValue>(byte[] bytes, out TValue value)
        {
            try
            {
                value = Deserialize<TValue>(bytes);
                return true;
            }
            catch (JsonException)
            {
                value = default;
                return false;
            }
        }

        public static JsonBodySerializer Default = new JsonBodySerializer(
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None
            });

    }
}
