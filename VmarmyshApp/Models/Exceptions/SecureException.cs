using System.Text.Json;
using System.Text.Json.Serialization;
using VmarmyshApp.Helpers;

namespace VmarmyshApp.Models.ExceptionsModels
{
    public class SecureException : Exception
    {
        [JsonInclude]
        public string Type { get; set; }

        [JsonInclude]
        public int Id { get; set; }

        [JsonInclude]
        public Data Data { get; set; }

        [JsonIgnore]
        public string DataString { get; set; }

        public SecureException() { }

        public SecureException(string message) : base(message)
        {
            Type = "Secure";
            Id = HResult;
            Data = new Data(message);
            DataString = Data.ToString();
        }

        public SecureException(Exception ex)
        {
            Type = "Exception";
            Id = ex.HResult;
            Data = new Data($"Internal server error ID = {Id}");
            DataString = Data.ToString();
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new SecureExceptionSerializer() }
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}