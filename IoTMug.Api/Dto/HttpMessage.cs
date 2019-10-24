namespace IoTMug.Api.Dto
{
    public class HttpMessage
    {
        public string Message { get; set; }

        public HttpMessage(string message) => Message = message;
    }
}
