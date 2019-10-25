namespace IoTMug.Api.Dto
{
    public class HttpMessageDto
    {
        public string Message { get; set; }

        public HttpMessageDto(string message) => Message = message;
    }
}
