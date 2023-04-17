namespace VmarmyshApp.Models.ExceptionsModels
{
    public class Data
    {
        public string Message { get; set; }

        public Data(string message)
        {
            Message = message;
        }

        public override string ToString() { return Message; }
    }
}