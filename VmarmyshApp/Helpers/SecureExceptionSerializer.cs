using System.Text.Json;
using System.Text.Json.Serialization;
using VmarmyshApp.Models.ExceptionsModels;

namespace VmarmyshApp.Helpers
{
    public class SecureExceptionSerializer : JsonConverter<SecureException>
    {
        public override SecureException Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, SecureException value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.Type);
            writer.WriteString("Id", value.Id.ToString());
            writer.WritePropertyName("Data");
            JsonSerializer.Serialize(writer, value.Data, options);
            writer.WriteEndObject();
        }
    }
}